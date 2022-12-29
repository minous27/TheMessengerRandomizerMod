using System;
using System.Collections.Generic;
using System.Linq;
using MessengerRando.RO;
using MessengerRando.Utils;

namespace MessengerRando.Archipelago
{
    public static class ItemsAndLocationsHandler
    {
        public static Dictionary<long, RandoItemRO> ItemsLookup = new Dictionary<long, RandoItemRO>();
        public static Dictionary<LocationRO, long> LocationsLookup { get; } = new Dictionary<LocationRO, long>();

        private static RandomizerStateManager randoStateManager;

        /// <summary>
        /// Builds the item and lookup dictionaries for converting to and from AP checks. Will always make every location
        /// a check, whether they are or not, for simplicity's sake.
        /// </summary>
        public static void Initialize()
        {
            const long baseOffset = 0xADD_000;
            
            long offset = baseOffset;
            Console.WriteLine("Building ItemsLookup...");
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
        
        public static void Unlock(long itemToUnlock, int quantity = 1)
        {
            if (!ItemsLookup.TryGetValue(itemToUnlock, out var randoItem))
            {
                Console.WriteLine($"Couldn't find {itemToUnlock} in items to grant it.");
                return;
            }

            if (EItems.WINDMILL_SHURIKEN.Equals(randoItem.Item)) RandomizerMain.OnToggleWindmillShuriken();
            else if (EItems.TIME_SHARD.Equals(randoItem.Item))
            {
                Manager<InventoryManager>.Instance.CollectTimeShard(quantity);
                randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItem);
                return; //Collecting timeshards internally call add item so I dont need to do it again.
            }
            randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItem);
            Manager<InventoryManager>.Instance.AddItem(randoItem.Item, quantity);
        }

        public static void SendLocationCheck(LocationRO checkedLocation)
        {
            ArchipelagoClient.ServerData.CheckedLocations.Add(checkedLocation);
            long checkID = LocationsLookup[checkedLocation];
            ArchipelagoClient.Session.Locations.CompleteLocationChecksAsync(checkID);
        }
    }
}