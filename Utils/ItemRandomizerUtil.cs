using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using Mod.Courier;
using Mod.Courier.Module;
using MessengerRando.RO;
using MessengerRando.Exceptions;


namespace MessengerRando.Utils
{
    //This class will be responsible for handling the randomization of items to locations and generating the mapping dictionary.
    public class ItemRandomizerUtil
    {
        //Used to represent all the required items to complete this seed, along with what they currently block. This is to prevent self locks. 
        private static Dictionary<RandoItemRO, HashSet<RandoItemRO>> requiredItems = new Dictionary<RandoItemRO, HashSet<RandoItemRO>>();

        //Checks to see if all expected notes have already been collected
        public static bool HasAllNotes()
        {
            if (Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_CHAOS) > 0
                && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_COURAGE) > 0
                && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_HOPE) > 0
                && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_LOVE) > 0
                && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_STRENGTH) > 0
                && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.KEY_OF_SYMBIOSIS) > 0)
            {
                return true;
            }
            return false;
        }

        //Get the version number, returns "Unknown" if it has trouble getting the version number.
        public static string GetModVersion()
        {
            string version = "Unknown";

            foreach (CourierModuleMetadata modMetadata in Courier.Mods)
            {
                if ("TheMessengerRandomizer".Equals(modMetadata.Name))
                {
                    version = modMetadata.VersionString;
                }
            }

            return version;
        }
        
        public static string LoadMappingsFromFile(int fileSlot)
        {
            //Get a handle on the necessary mappings file
            Console.WriteLine($"Attempting to load mappings from file for file slot '{fileSlot}'");
            return File.ReadAllText($@"Mods\TheMessengerRandomizer\Mappings\MessengerRandomizerMapping_{fileSlot}.txt");
        }

        //I want to simulate running through a seed and checking all I can with what Items I have.
        public static bool IsSeedBeatable(Dictionary<LocationRO, RandoItemRO> mappings)
        {
            //Create an player that will be used to track progress.
            SamplePlayerRO player = new SamplePlayerRO(false, false, false, 0, new List<RandoItemRO>());

            //I'll want to start doing runs through the mappings to see if I am able to collect items. I'll keep doing this until I either get the 6 notes or I have no more checks I can do.
            while (player.NoteCount < 6)
            {
                //Create a copy of the mappings to mess with
                Dictionary<LocationRO, RandoItemRO> localMappings = new Dictionary<LocationRO, RandoItemRO>(mappings);

                bool collectedItemThisRound = false;
                //Run through locations, get any items we can
                foreach (LocationRO location in localMappings.Keys)
                {
                    //Lets check additional items and get that over with first
                    EItems[] additionalLocationRequiredItems = location.AdditionalRequiredItemsForCheck;

                    if (!additionalLocationRequiredItems.Contains(EItems.NONE))
                    {
                        {
                            //There are additional items to check
                            if (!HasAdditionalItemsForBeatableSeedCheck(additionalLocationRequiredItems, player))
                            {
                                //Did not pass validations, move to next location.
                                continue;
                            }
                        }
                    }

                    //Start the fun location checks
                    if (location.IsWingsuitRequired)
                    {
                        //Wingsuit check
                        if (!player.HasWingsuit)
                        {
                            continue;
                        }
                    }
                    //Ropedart check
                    if (location.IsRopeDartRequired)
                    {
                        if (!player.HasRopeDart)
                        {
                            continue;
                        }
                    }
                    //Ninja Tabi check
                    if (location.IsNinjaTabiRequired)
                    {
                        if (!player.HasNinjaTabis)
                        {
                            continue;
                        }
                    }
                    //Checks that could use either rope dart or wingsuit
                    if (location.IsEitherWingsuitOrRopeDartRequired)
                    {
                        if (!player.HasWingsuit && !player.HasRopeDart)
                        {
                            continue;
                        }
                    }

                    //If we survived all of that nonsense, then we passed validations. The item is ours!
                    CollectItemForBeatableSeedCheck(localMappings[location], ref player);
                    collectedItemThisRound = true;
                    mappings.Remove(location);
                }
                if (!collectedItemThisRound)
                {
                    //This seed not beatable
                    Console.WriteLine("\nSeed was deemed unbeatable.");
                    //Print out all the items collected so far
                    Console.WriteLine($"Note Count: {player.NoteCount}");
                    Console.WriteLine($"Collected Wingsuit: {player.HasWingsuit}");
                    Console.WriteLine($"Collected Ropedart: {player.HasRopeDart}");
                    Console.WriteLine($"Collected Ninja Tabis: {player.HasNinjaTabis}");

                    foreach (RandoItemRO additionalItem in player.AdditionalItems)
                    {
                        Console.WriteLine($"Additional Item Collected: {additionalItem}");
                    }
                    

                    //Print out remaining locations
                    Console.WriteLine("\nRemaining location mappings:");

                    foreach(LocationRO location in mappings.Keys)
                    {
                        Console.WriteLine($"Location: '{location.PrettyLocationName}' | Item at location: '{mappings[location].Name}'");
                    }

                    return false;
                }
            }

            //We made it through the game with all 6 notes!
            Console.WriteLine("Mapping successfully verified. This seed is beatable.");
            return true;
        }

        public static string DecryptSeedInfo(string b64SeedInfo)
        {
            //We'll need to take the b64 string and decrypt it so we can get the seed info.

            byte[] bytes = Convert.FromBase64String(b64SeedInfo);

            string seedInfo = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"Decoded seed info string: '{seedInfo}'");

            return seedInfo;
        }

        public static SeedRO ParseSeed(int fileSlot, string seedInfo)
        {


            //Break up mapping string
            string[] fullSeedInfoArr = seedInfo.Split('|');

            string mappingText = fullSeedInfoArr[0].Substring(fullSeedInfoArr[0].IndexOf('=') + 1);
            Console.WriteLine($"Mapping text: '{mappingText}'");

            string settingsText = fullSeedInfoArr[1];
            Console.WriteLine($"Settings text: '{settingsText}'");

            string seedTypeText = fullSeedInfoArr[2];
            Console.WriteLine($"Seed Type text: '{seedTypeText}'");

            string seedNumStr = fullSeedInfoArr[3];
            Console.WriteLine($"Seed Number text: '{seedTypeText}'");

            
            //Settings
            Dictionary<SettingType, SettingValue> settings = new Dictionary<SettingType, SettingValue>();
            string[] settingsArr = settingsText.Split(',');

            foreach (string setting in settingsArr)
            {
                Console.WriteLine($"Settings - Working with: '{setting}'");
                string[] settingKV = setting.Split('=');
                settings.Add((SettingType) Enum.Parse(typeof(SettingType), settingKV[0]), (SettingValue) Enum.Parse(typeof(SettingValue), settingKV[1]));
            }

            //Seedtype
            string seedTypeStr = seedTypeText.Substring(seedTypeText.IndexOf('=') + 1);
            SeedType seedType = (SeedType)Enum.Parse(typeof(SeedType), seedTypeStr);

            //Seed Number

            int seedNum = Int32.Parse(seedNumStr.Substring(seedNumStr.IndexOf('=') + 1));

            return new SeedRO(fileSlot,seedType,seedNum, settings, null, mappingText);
        }

        public static Dictionary<LocationRO, RandoItemRO> ParseLocationToItemMappings(SeedRO seed)
        {
            //Prep
            Dictionary<LocationRO, RandoItemRO> mappings = new Dictionary<LocationRO, RandoItemRO>();
            Dictionary<string, LocationRO> officialLocations = new Dictionary<string, LocationRO>();
            Dictionary<string, RandoItemRO> officialItems = new Dictionary<string, RandoItemRO>();

            //Fill official collections for easy searching
            foreach(LocationRO location in RandomizerConstants.GetRandoLocationList())
            {
                officialLocations.Add(location.LocationName, location);
            }

            foreach(LocationRO location in RandomizerConstants.GetAdvancedRandoLocationList())
            {
                officialLocations.Add(location.LocationName, location);
            }

            foreach(RandoItemRO item in RandomizerConstants.GetRandoItemList())
            {
                officialItems.Add(item.Name, item);
            }

            foreach (RandoItemRO item in RandomizerConstants.GetNotesList())
            {
                officialItems.Add(item.Name, item);
            }

            //Split up all the mappings
            string[] mappingsArr = seed.MappingInfo.Split(',');

            foreach(string mappingStr in mappingsArr)
            {
                //Split off the location and item string
                string[] mappingArr = mappingStr.Split('-');
                LocationRO location = null;
                RandoItemRO item = new RandoItemRO();


                //Get the LocationRO and RandoItemRO from the list of known items
                if(officialLocations.ContainsKey(mappingArr[0]))
                {
                    location = officialLocations[mappingArr[0]];
                }
                else
                {
                    //If for some reason something that could not be mapped to an official location, let's fail for now.
                    throw new RandomizerException($"Location named '{mappingArr[0]}' could not be located in collection of official locations.");
                }

                if (officialItems.ContainsKey(mappingArr[1]))
                {
                    item = officialItems[mappingArr[1]];
                }
                else
                {
                    //If for some reason something that could not be mapped to an official location, let's fail for now.
                    throw new RandomizerException($"Item named '{mappingArr[1]}' could not be located in collection of official items.");
                }

                //We get here, then we are good. Save off this mapping and move on.
                mappings.Add(location, item);

            }

            Console.WriteLine("Mapping parsed successfully!");
            return mappings;
        }

        private static bool HasAdditionalItemsForBeatableSeedCheck(EItems[] additionalLocationRequiredItems, SamplePlayerRO player)
        {
            bool hasAdditionalItems = true;

            //Check each item in the list of required items for this location
            foreach (EItems item in additionalLocationRequiredItems)
            {
                bool itemFound = false;
                //Check each item the player has
                foreach (RandoItemRO playerItem in player.AdditionalItems)
                {
                    if (playerItem.Item.Equals(item))
                    {
                        //We have this required item
                        itemFound = true;
                        break;
                    }
                }
                if (!itemFound)
                {
                    //We were missing at least one required item
                    hasAdditionalItems = false;
                    break;
                }
            }
            return hasAdditionalItems;
        }

        private static void CollectItemForBeatableSeedCheck(RandoItemRO itemToCollect, ref SamplePlayerRO player)
        {
            //Handle the various types of items
            switch(itemToCollect.Item)
            {
                case EItems.WINGSUIT:
                    player.HasWingsuit = true;
                    break;
                case EItems.GRAPLOU:
                    player.HasRopeDart = true;
                    break;
                case EItems.MAGIC_BOOTS:
                    player.HasNinjaTabis = true;
                    break;
                case EItems.KEY_OF_CHAOS:
                    player.NoteCount++;
                    break;
                case EItems.KEY_OF_COURAGE:
                    player.NoteCount++;
                    break;
                case EItems.KEY_OF_HOPE:
                    player.NoteCount++;
                    break;
                case EItems.KEY_OF_LOVE:
                    player.NoteCount++;
                    break;
                case EItems.KEY_OF_STRENGTH:
                    player.NoteCount++;
                    break;
                case EItems.KEY_OF_SYMBIOSIS:
                    player.NoteCount++;
                    break;
                default:
                    //Some other item, just throw it in
                    player.AdditionalItems.Add(itemToCollect);
                    break;
            }
        }

        private static void RecursiveBlockedItemCheck(HashSet<RandoItemRO> recursiveBlockedItems, ref HashSet<RandoItemRO> blockerItems)
        {
            foreach(RandoItemRO recursiveBlockedItem in recursiveBlockedItems)
            {
                //There are situations where a few items might block each other. I'm putting something here to protect against an infinite loop for now.
                if (blockerItems.Contains(recursiveBlockedItem))
                {
                    //No need to add and look through again this run
                    return;
                }

                //tempRequiredItems[origItem].Add(recursiveBlockedItem);
                blockerItems.Add(recursiveBlockedItem);

                HashSet<RandoItemRO> evenMoreRecursiveBlockedItems = new HashSet<RandoItemRO>();
                

                if(requiredItems.TryGetValue(recursiveBlockedItem, out evenMoreRecursiveBlockedItems))
                {
                    RecursiveBlockedItemCheck(evenMoreRecursiveBlockedItems, ref blockerItems);
                }
            }
        }
    }
}
