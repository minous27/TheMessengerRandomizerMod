using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using MessengerRando.RO;
using Newtonsoft.Json;

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

        public static void ConnectAsync(string uri, int port, string slotName, string password)
        {
            if (ServerData == null || !HasConnected)
            {
                ServerData = new ArchipelagoData
                {
                    Uri = uri,
                    Port = port,
                    SlotName = slotName,
                    Password = password
                };
            }
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
                if (ServerData.SlotData == null) ServerData.SlotData = success.SlotData;
                Authenticated = true;
                if (HasConnected)
                {
                    for (int i = Session.Locations.AllLocationsChecked.Count; i < ServerData.CheckedLocations.Count; i++)
                    {
                        Session.Locations.CompleteLocationChecksAsync(
                            ItemsAndLocationsHandler.LocationsLookup[ServerData.CheckedLocations[i]]
                            );
                    }
                    return;
                }
                ItemsAndLocationsHandler.Initialize();

                //Locally placed items from AP get added to the slot data so add those to the mapping here so we can
                //reward our own items. Locations with items for other players will just be empty
                if (ServerData.SlotData.TryGetValue("locations", out var locations))
                {
                    var localItems = JsonConvert.DeserializeObject<Dictionary<string, long>>((string)locations);
                    foreach (LocationRO location in ItemsAndLocationsHandler.LocationsLookup.Keys)
                    {
                        RandoItemRO item;
                        item = localItems.TryGetValue(location.LocationName, out var itemID)
                            ? ItemsAndLocationsHandler.ItemsLookup[itemID]
                            : new RandoItemRO();
                        RandomizerStateManager.Instance.CurrentLocationToItemMapping.Add(location, item);
                    }
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

        private static void Disconnect()
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
                
                ConnectAsync(ServerData.Uri, ServerData.Port, ServerData.SlotName, ServerData.Password);
                disconnectTimeout = 5;
                return;
            }
            if (ServerData.Index >= Session.Items.AllItemsReceived.Count) return;
            var currentItemId = Session.Items.AllItemsReceived[Convert.ToInt32(ServerData.Index)].Item;
            ++ServerData.Index;
            ItemsAndLocationsHandler.Unlock(currentItemId);
        }
    }
}