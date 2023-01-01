using System;
using System.Collections.Generic;
using System.Linq;
using MessengerRando.RO;
using MessengerRando.Utils;
using UnityEngine;

namespace MessengerRando.Archipelago
{
    public static class ItemsAndLocationsHandler
    {
        public static Dictionary<long, RandoItemRO> ItemsLookup;
        public static Dictionary<LocationRO, long> LocationsLookup;

        private static RandomizerStateManager randoStateManager;

        public const int APQuantity = 69;

        /// <summary>
        /// Builds the item and lookup dictionaries for converting to and from AP checks. Will always make every location
        /// a check, whether they are or not, for simplicity's sake.
        /// </summary>
        public static void Initialize()
        {
            const long baseOffset = 0xADD_000;

            long offset = baseOffset;
            Console.WriteLine("Building ItemsLookup...");
            ItemsLookup = new Dictionary<long, RandoItemRO>();
            foreach (var note in RandomizerConstants.GetNotesList())
            {
                ItemsLookup.Add(offset, note);
                ++offset;
            }
            foreach (var item in RandomizerConstants.GetRandoItemList())
            {
                ItemsLookup.Add(offset, item);
                ++offset;
            }
            foreach (var shard in RandomizerConstants.GetAdvancedRandoItemList())
            {
                ItemsLookup.Add(offset, shard);
                ++offset;
            }

            offset = baseOffset;
            Console.WriteLine("Building LocationsLookup...");
            LocationsLookup = new Dictionary<LocationRO, long>();
            foreach (var progLocation in RandomizerConstants.GetRandoLocationList())
            {
                LocationsLookup.Add(progLocation, offset);
                ++offset;
            }
            foreach (var advLocation in RandomizerConstants.GetAdvancedRandoLocationList())
            {
                LocationsLookup.Add(advLocation, offset);
                ++offset;
            }

            randoStateManager = RandomizerStateManager.Instance;
        }
        
        /// <summary>
        /// We received an item from the server so add it to our inventory. Set the quantity to an absurd number here so we can differentiate.
        /// </summary>
        /// <param name="itemToUnlock"></param>
        /// <param name="quantity"></param>
        public static void Unlock(long itemToUnlock, int quantity = APQuantity)
        {
            if (!ItemsLookup.TryGetValue(itemToUnlock, out var randoItem))
            {
                Console.WriteLine($"Couldn't find {itemToUnlock} in items to grant it.");
                return;
            }

            if (EItems.WINDMILL_SHURIKEN.Equals(randoItem.Item)) RandomizerMain.OnToggleWindmillShuriken();
            else if (EItems.TIME_SHARD.Equals(randoItem.Item))
            {
                Console.WriteLine("Unlocking time shards...");
                Manager<InventoryManager>.Instance.CollectTimeShard(quantity);
                randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItem);
                return; //Collecting timeshards internally call add item so I dont need to do it again.
            }
            Console.WriteLine($"Adding {randoItem.Name} to inventory...");
            randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItem);
            ArchipelagoClient.ServerData.ReceivedItems.Add(randoItem);
            Manager<InventoryManager>.Instance.AddItem(randoItem.Item, quantity);
        }

        public static void SendLocationCheck(LocationRO checkedLocation)
        {
            Console.WriteLine($"Player found item at {checkedLocation.PrettyLocationName}");
            long checkID = LocationsLookup[checkedLocation];
            ArchipelagoClient.ServerData.CheckedLocations.Add(checkID);
            ArchipelagoClient.Session.Locations.CompleteLocationChecks(checkID);
            ArchipelagoClient.ServerData.UpdateSave();
        }
    }
}