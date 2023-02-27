using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using MessengerRando.RO;
using MessengerRando.Utils;
using SonicBloom.MIDI.Objects;
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

            foreach (var item in ArchipelagoItems)
            {
                ItemsLookup.Add(offset, item);
                ++offset;
            }

            offset = baseOffset;
            Console.WriteLine("Building LocationsLookup...");
            LocationsLookup = new Dictionary<LocationRO, long>();
            foreach (var progLocation in ArchipelagoLocations)
            {
                LocationsLookup.Add(progLocation, offset);
                ++offset;
            }

            randoStateManager = RandomizerStateManager.Instance;
        }

        public static List<RandoItemRO> ArchipelagoItems = new List<RandoItemRO>()
        {
            //notes
            new RandoItemRO("Key_Of_Hope", EItems.KEY_OF_HOPE),
            new RandoItemRO("Key_Of_Chaos", EItems.KEY_OF_CHAOS),
            new RandoItemRO("Key_Of_Courage", EItems.KEY_OF_COURAGE),
            new RandoItemRO("Key_Of_Love", EItems.KEY_OF_LOVE),
            new RandoItemRO("Key_Of_Strength", EItems.KEY_OF_STRENGTH),
            new RandoItemRO("Key_Of_Symbiosis", EItems.KEY_OF_SYMBIOSIS),
            //upgrades
            new RandoItemRO("Windmill_Shuriken", EItems.WINDMILL_SHURIKEN),
            new RandoItemRO("Wingsuit", EItems.WINGSUIT),
            new RandoItemRO("Rope_Dart", EItems.GRAPLOU),
            new RandoItemRO("Ninja_Tabis", EItems.MAGIC_BOOTS),
            //quest items
            //new RandoItemRO("Astral_Seed", EItems.TEA_SEED),
            //new RandoItemRO("Astral_Tea_Leaves", EItems.TEA_LEAVES),
            new RandoItemRO("Candle", EItems.CANDLE),
            new RandoItemRO("Seashell", EItems.SEASHELL),
            new RandoItemRO("Power_Thistle", EItems.POWER_THISTLE),
            new RandoItemRO("Demon_King_Crown", EItems.DEMON_KING_CROWN),
            new RandoItemRO("Ruxxtin_Amulet", EItems.RUXXTIN_AMULET),
            new RandoItemRO("Fairy_Bottle", EItems.FAIRY_BOTTLE),
            new RandoItemRO("Sun_Crest", EItems.SUN_CREST),
            new RandoItemRO("Moon_Crest", EItems.MOON_CREST),
            //Phobekins
            new RandoItemRO("Necro", EItems.NECROPHOBIC_WORKER),
            new RandoItemRO("Pyro", EItems.PYROPHOBIC_WORKER),
            new RandoItemRO("Claustro", EItems.CLAUSTROPHOBIC_WORKER),
            new RandoItemRO("Acro", EItems.ACROPHOBIC_WORKER),
            //Power seals - not sure if i can make these work
            new RandoItemRO("PowerSeal", EItems.POWER_SEAL),
            //time shards
            new RandoItemRO("Timeshard", EItems.TIME_SHARD),
        };

        public static List<LocationRO> ArchipelagoLocations = new List<LocationRO>()
        {
            //notes
            new LocationRO("Key_Of_Love", EItems.KEY_OF_LOVE),
            new LocationRO("Key_Of_Courage", EItems.KEY_OF_COURAGE),
            new LocationRO("Key_Of_Chaos", EItems.KEY_OF_CHAOS),
            new LocationRO("Key_Of_Symbiosis", EItems.KEY_OF_SYMBIOSIS),
            new LocationRO("Key_Of_Strength", EItems.KEY_OF_STRENGTH),
            new LocationRO("Key_Of_Hope", EItems.KEY_OF_HOPE),
            //upgrades
            new LocationRO("Wingsuit", EItems.WINGSUIT),
            new LocationRO("Rope_Dart", EItems.GRAPLOU),
            new LocationRO("Ninja_Tabis", EItems.MAGIC_BOOTS),
            new LocationRO("Climbing_Claws", EItems.CLIMBING_CLAWS),
            //quest items
            new LocationRO("Astral_Seed", EItems.TEA_SEED),
            new LocationRO("Astral_Tea_Leaves", EItems.TEA_LEAVES),
            new LocationRO("Candle", EItems.CANDLE),
            new LocationRO("Seashell", EItems.SEASHELL),
            new LocationRO("Power_Thistle", EItems.POWER_THISTLE),
            new LocationRO("Demon_King_Crown", EItems.DEMON_KING_CROWN),
            new LocationRO("Ruxxtin_Amulet", EItems.RUXXTIN_AMULET),
            new LocationRO("Fairy_Bottle", EItems.FAIRY_BOTTLE),
            new LocationRO("Sun_Crest", EItems.SUN_CREST),
            new LocationRO("Moon_Crest", EItems.MOON_CREST),
            //Phobekins
            new LocationRO("Necro", EItems.NECROPHOBIC_WORKER),
            new LocationRO("Pyro", EItems.PYROPHOBIC_WORKER),
            new LocationRO("Claustro", EItems.CLAUSTROPHOBIC_WORKER),
            new LocationRO("Acro", EItems.ACROPHOBIC_WORKER),
            //power seals
            //Ninja Village
            new LocationRO("-436-404-44-28", "Ninja Village Seal - Tree House"),
            //Autumn Hills
            new LocationRO("-52-20-60-44", "Autumn Hills Seal - Trip Saws"),
            new LocationRO("556588-44-28", "Autumn Hills Seal - Double Swing Saws"),
            new LocationRO("748780-76-60", "Autumn Hills Seal - Spike Ball Swing"),
            new LocationRO("748780-108-76", "Autumn Hills Seal - Spike Ball Darts"),
            //Catacombs
            new LocationRO("236268-44-28", "Catacombs Seal - Triple Spike Crushers"),
            new LocationRO("492524-44-28", "Catacombs Seal - Crusher Gauntlet"),
            new LocationRO("556588-60-44", "Catacombs Seal - Dirty Pond"),
            //Bamboo Creek
            new LocationRO("-84-52-28-12", "Bamboo Creek Seal - Spike crushers and Doors"),
            new LocationRO("172236-44-28", "Bamboo Creek Seal - Spike ball pits"),
            new LocationRO("300332-1236", "Bamboo Creek Seal - Spike crushers and Doors v2"),
            //Howling Grotto
            new LocationRO("108140-28-12", "Howling Grotto Seal - Windy Saws and Balls"),
            new LocationRO("300332-92-76", "Howling Grotto Seal - Crushing Pits"),
            new LocationRO("460492-172-156", "Howling Grotto Seal - Breezy Crushers"),
            //Quillshroom Marsh
            new LocationRO("204236-28-12", "Quillshroom Marsh Seal - Spikey Window"),
            new LocationRO("652684-60-28", "Quillshroom Marsh Seal - Sand Trap"),
            new LocationRO("940972-28-12", "Quillshroom Marsh Seal - Do the Spike Wave"),
            //Searing Crags
            new LocationRO("761085268", "Searing Crags Seal - Triple Ball Spinner"),
            new LocationRO("300332196212", "Searing Crags Seal - Raining Rocks"),
            new LocationRO("364396292308", "Searing Crags Seal - Rythym Rocks"),
            //Glacial Peak
            new LocationRO("140172-492-476", "Glacial Peak Seal - Ice Climbers"),
            new LocationRO("236268-396-380", "Glacial Peak Seal - Projectile Spike Pit"),
            new LocationRO("236268-156-140", "Glacial Peak Seal - Glacial Air Swag"),
            //TowerOfTime
            new LocationRO("-84-522036", "TowerOfTime Seal - Time Waster Seal"),
            new LocationRO("7610852116", "TowerOfTime Seal - Lantern Climb"),
            new LocationRO("-52-20116132", "TowerOfTime Seal - Arcane Orbs"),
            //Cloud Ruins
            new LocationRO("-148-116420", "Cloud Ruins Seal - Ghost Pit"),
            new LocationRO("108140-44-28", "Cloud Ruins Seal - Toothbrush Alley"),
            new LocationRO("748780-44-28", "Cloud Ruins Seal - Saw Pit"),
            new LocationRO("11321164-124", "Cloud Ruins Seal - Money Farm Room"),
            //Underworld
            new LocationRO("-276-244-444", "Underworld Seal - Sharp and Windy Climb"),
            new LocationRO("-180-148-44-28", "Underworld Seal - Spike Wall"),
            new LocationRO("-180-148-60-44", "Underworld Seal - Fireball Wave"),
            new LocationRO("-2012-124", "Underworld Seal - Rising Fanta"),
            //Forlorn Temple
            new LocationRO("172268-284", "Forlorn Temple Seal - Rocket Maze"),
            new LocationRO("140172100164", "Forlorn Temple Seal - Rocket Sunset"),
            //Sunken Shrine
            new LocationRO("204236-124", "Sunken Shrine Seal - Ultra Lifeguard"),
            new LocationRO("172268-188-172", "Sunken Shrine Seal - Waterfall Paradise"),
            new LocationRO("-148-116-124-60", "Sunken Shrine Seal - Tabi Gauntlet"),
            //Riviere Turquoise
            new LocationRO("844876-284", "Reviere Turquoise Seal - Bounces and Balls"),
            new LocationRO("460492-124-108", "Reviere Turquoise Seal - Launch of Faith"),
            new LocationRO("-180-1483668", "Reviere Turquoise Seal - Flower Power"),
            //Elemental Skylands
            new LocationRO("-52-20420436", "Elemental Skylands Seal - Air Seal"),
            new LocationRO("18361868372388", "Elemental Skylands Seal - Water Seal"),
            new LocationRO("28602892356388", "Elemental Skylands Seal - Fire Seal"),
        };
        
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

            switch (randoItem.Item)
            {
                case EItems.WINDMILL_SHURIKEN:
                    RandomizerMain.OnToggleWindmillShuriken();
                    break;
                case EItems.TIME_SHARD:
                    Console.WriteLine("Unlocking time shards...");
                    Manager<InventoryManager>.Instance.CollectTimeShard(quantity);
                    break;
                case EItems.POWER_SEAL:
                    randoStateManager.PowerSealManager.AddPowerSeal();
                    break;
                default:
                    Manager<InventoryManager>.Instance.AddItem(randoItem.Item, quantity);
                    break;
            }
            randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItem);
        }

        public static void SendLocationCheck(LocationRO checkedLocation)
        {
            Console.WriteLine($"Player found item at {checkedLocation.PrettyLocationName}");
            long checkID = LocationsLookup[checkedLocation];
            ArchipelagoClient.ServerData.CheckedLocations.Add(checkID);
            if (ArchipelagoClient.Authenticated)
                ArchipelagoClient.Session.Locations.CompleteLocationChecks(checkID);
        }
    }
}