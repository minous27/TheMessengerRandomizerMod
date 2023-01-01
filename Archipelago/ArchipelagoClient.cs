using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;
using MessengerRando.Utils;
using UnityEngine;

namespace MessengerRando.Archipelago
{
    public static class ArchipelagoClient
    {
        private const string ApVersion = "0.3.7";
        public static ArchipelagoData ServerData;

        //for timers to attempt to reconnect if connection is lost
        private static int lastAttemptTime;
        private static int disconnectTimeout = 5;

        public static bool Authenticated;
        public static bool HasConnected;

        public static ArchipelagoSession Session;
        public static DeathLinkService DeathLinkService;

        public static void ConnectAsync()
        {
            if (ServerData == null)
            {
                ServerData = new ArchipelagoData();
                return;
            }
            ItemsAndLocationsHandler.Initialize();
            Console.WriteLine($"Connecting to {ServerData.Uri}:{ServerData.Port} as {ServerData.SlotName}");
            ThreadPool.QueueUserWorkItem(_ => Connect());
        }

        private static void Connect()
        {
            if (Authenticated) return;

            LoginResult result;

            Session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri, ServerData.Port);
            Session.MessageLog.OnMessageReceived += OnMessageReceived;
            Session.Socket.ErrorReceived += SessionErrorReceived;
            Session.Socket.SocketClosed += SessionSocketClosed;

            try
            {
                Console.WriteLine($"Attempting Connection...");
                result = Session.TryConnectAndLogin(
                    "The Messenger",
                    ServerData.SlotName,
                    ItemsHandlingFlags.AllItems,
                    new Version(ApVersion),
                    null,
                    "",
                    ServerData.Password == "" ? null : ServerData.Password
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.GetBaseException().Message}");
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (result.Successful)
            {
                var success = (LoginSuccessful)result;
                ServerData.SlotData = success.SlotData;
                ServerData.SeedName = Session.RoomState.Seed;
                Authenticated = true;
                if (HasConnected)
                {
                    for (int i = Session.Locations.AllLocationsChecked.Count; i < ServerData.CheckedLocations.Count; i++)
                    {
                        Session.Locations.CompleteLocationChecks(ServerData.CheckedLocations[i]);
                    }
                    return;
                }
                ServerData.UpdateSave();
                HasConnected = true;
            }
            else
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}:";
                errorMessage +=
                    failure.Errors.Aggregate(errorMessage, (current, error) => current + $"\n    {error}");
                errorMessage +=
                    failure.ErrorCodes.Aggregate(errorMessage, (current, error) => current + $"\n   {error}");

                Console.WriteLine($"Failed to connect: {errorMessage}");

                Authenticated = false;
                Disconnect();
            }
        }

        private static void OnMessageReceived(LogMessage message)
        {
            Console.WriteLine(message.ToString());
        }

        private static void SessionErrorReceived(Exception e, string message)
        {
            Console.WriteLine(message);
            Console.WriteLine(e.GetBaseException().Message);
        }

        private static void SessionSocketClosed(string reason) 
        {
            Console.WriteLine($"Connection to Archipelago lost: {reason}");
            Disconnect();
        }

        public static void Disconnect()
        {
            Session?.Socket.DisconnectAsync();
            Session = null;
            Authenticated = false;
        }

        public static void UpdateArchipelagoState()
        {
            if (!Authenticated)
            {
                var now = DateTime.Now.Second;
                var dT = now - lastAttemptTime;
                lastAttemptTime = now;
                disconnectTimeout -= dT;
                if (!(disconnectTimeout <= 0.0f)) return;
                
                ConnectAsync();
                disconnectTimeout = 5;
                return;
            }
            if (ServerData.Index >= Session.Items.AllItemsReceived.Count) return;
            var currentItem = Session.Items.AllItemsReceived[Convert.ToInt32(ServerData.Index)];
            var currentItemId = currentItem.Item;
            ++ServerData.Index;
            ItemsAndLocationsHandler.Unlock(currentItemId);
            ServerData.UpdateSave();
            if (!currentItem.Player.Equals(Session.ConnectionInfo.Slot))
            {
                DialogSequence receivedItem = ScriptableObject.CreateInstance<DialogSequence>();
                receivedItem.dialogID = "RANDO_ITEM";
                receivedItem.name = Session.Items.GetItemName(currentItemId);
                receivedItem.choices = new List<DialogSequenceChoice>();
                AwardItemPopupParams receivedItemParams = new AwardItemPopupParams(receivedItem, true);
                Manager<UIManager>.Instance.ShowView<AwardItemPopup>(EScreenLayers.PROMPT, receivedItemParams, true);
            }
        }

        public static void UpdateClientStatus(ArchipelagoClientState newState)
        {
            Console.WriteLine($"Updating client status to {newState}");
            var statusUpdatePacket = new StatusUpdatePacket() { Status = newState };
            if (ArchipelagoClientState.ClientGoal.Equals(newState))
                Session.DataStorage[Scope.Slot, "HasFinished"] = true;
            Session.Socket.SendPacket(statusUpdatePacket);
        }

        private static bool ClientFinished()
        {
            if (!Authenticated) return false;
            return Session.DataStorage[Scope.Slot, "HasFinished"].To<bool?>() == true;
        }

        public static bool CanRelease()
        {
            if (Authenticated)
            {
                Permissions releasePermission = Session.RoomState.ReleasePermissions;
                switch (releasePermission)
                {
                    case Permissions.Goal:
                        return ClientFinished();
                    case Permissions.Enabled:
                        return true;
                }
            }
            return false;
        }

        public static bool CanCollect()
        {
            if (Authenticated)
            {
                Permissions collectPermission = Session.RoomState.CollectPermissions;
                switch (collectPermission)
                {
                    case Permissions.Goal:
                        return ClientFinished();
                    case Permissions.Enabled:
                        return true;
                }
            }
            return false;
        }

        private static int GetHintCost()
        {
            var hintCost = Session.RoomState.HintCost;
            if (hintCost > 0)
            {
                RandomizerStateManager stateManager = RandomizerStateManager.Instance;
                int locationCount = RandomizerConstants.GetRandoLocationList().Count();
                if (SettingValue.Basic.Equals(ServerData.GameSettings[SettingType.Difficulty]))
                {
                    hintCost = locationCount / hintCost;
                }
                else
                {
                    locationCount += RandomizerConstants.GetAdvancedRandoLocationList().Count();
                    hintCost = locationCount / hintCost;
                }
            }
            return hintCost;
        }

        public static bool CanHint()
        {
            bool canHint = false;
            if (Authenticated)
            {
                canHint = GetHintCost() <= Session.RoomState.HintPoints;
            }
            return canHint;
        }

        public static string UpdateMenuText()
        {
            string text = string.Empty;
            if (Authenticated)
            {
                text = $"Connected to Archipelago server v{Session.RoomState.Version}";
                var hintCost = GetHintCost();
                if (hintCost > 0)
                {
                    text += $"\nHint points available: {Session.RoomState.HintPoints}\nHint point cost: {hintCost}";
                }
            }
            else if (HasConnected)
            {
                text = "Disconnected from Archipelago server.";
            }
            return text;
        }
    }
}