using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;

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
                    ItemsHandlingFlags.IncludeStartingInventory,
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
            var currentItemId = Session.Items.AllItemsReceived[Convert.ToInt32(ServerData.Index)].Item;
            ++ServerData.Index;
            ItemsAndLocationsHandler.Unlock(currentItemId);
            ServerData.UpdateSave();
        }

        public static string UpdateMenuText()
        {
            string text = string.Empty;
            if (Authenticated)
            {
                text = $"Connected to Archipelago server v{Session.RoomState.Version}\nSeed: {ServerData.SeedName}";
            }
            else if (HasConnected)
            {
                text = "Disconnected from Archipelago server.";
            }
            return text;
        }
    }
}