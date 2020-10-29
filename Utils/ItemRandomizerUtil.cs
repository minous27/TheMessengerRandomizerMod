using System;
using System.Collections.Generic;
using System.Linq;
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

        //private static SeedRO officialSeed;

        private static Random randomNumberGen;
        private static List<LocationRO> randomizedLocations;
        private static List<EItems> randomizedItems;
        private static Dictionary<EItems, HashSet<EItems>> requiredItemsWithBlockers;
        private static Dictionary<LocationRO, int> coinResults;

        private static int REQUIRED_ITEM_PLACEMENT_ATTEMPT_LIMIT = 10; //To prevent infinte loops from bugs in the item placement code

        public static Dictionary<LocationRO, EItems> GenerateRandomizedMappings(SeedRO seed)
        {
            //If no seed was provided, create one
            if(SeedType.None == seed.SeedType)
            {
                int tempSeed = GenerateSeed();

                if (seed.Seed == tempSeed)
                {
                    Console.WriteLine("Generated the same seed again. Will wait a bit and try again.");
                    Thread.Sleep(1000);//Wait a second
                    tempSeed = GenerateSeed();
                    //Do the check one more time and if nothing was fixed then log it.
                    if(seed.Seed == tempSeed)
                    {
                        Console.WriteLine("2 attempts to get a new seed failed. Moving along...");
                    }
                }

                seed = new SeedRO(SeedType.No_Logic, tempSeed, null);
                
                
                Console.WriteLine($"No seed passed, generated seed for this mapping is: {tempSeed}");
            }

            Console.WriteLine($"Beginning mapping generation for seed '{seed.Seed}'.");
            //We now have a seed. Let's initialize our locations and items lists.
            randomizedLocations = RandomizerConstants.GetRandoLocationList();
            randomizedItems = new List<EItems>(RandomizerConstants.randomizedItems);
            randomizedItems.AddRange(RandomizerConstants.notes);
            requiredItemsWithBlockers = new Dictionary<EItems, HashSet<EItems>>();
            coinResults = new Dictionary<LocationRO, int>();

            //Difficulty setting - if this is an advanced seed, at the other items and checks into the fray
            if(seed.Settings.ContainsKey(SettingType.Difficulty) && SettingValue.Advanced.Equals(seed.Settings[SettingType.Difficulty]))
            {
                //Advanced difficulty seed
                randomizedLocations.AddRange(RandomizerConstants.GetAdvancedRandoLocationList());
                randomizedItems.AddRange(RandomizerConstants.randomizedAdvancedItems);
            }

            //Get our randomizer set up
            randomNumberGen = new Random(seed.Seed);

            //Begin filling out the mappings. Both collections need to logically be the same size.
            if (randomizedLocations.Count != randomizedItems.Count)
            {
                //This check is here to make sure nothing was missed during development and check/item counts remain consistent. This should never break during typical usage and should only happen when changes to the logic engine are occurring.
                throw new RandomizerException($"Mismatched number of items between randomized items({randomizedItems.Count}) and checks({randomizedLocations.Count}). Minous needs to correct this so the world can work again...");
            }

            Dictionary<LocationRO, EItems> mappings = new Dictionary<LocationRO, EItems>();

            //Let the mapping flows begin!
            switch(seed.SeedType)
            {
                case SeedType.No_Logic:
                    //No logic, fast map EVERYTHING!
                    FastMapping(randomizedItems, ref mappings);
                    Console.WriteLine("No-Logic mapping generation complete.");
                    break;
                case SeedType.Logic:
                    //Basic logic. Start by placing the notes then do the logic things!
                    FastMapping(new List<EItems>(RandomizerConstants.notes), ref mappings);
                    //Now that the notes have a home, lets get all the items we are going to need to collect them. We will do this potentially a few times to ensure that all required items are accounted for.
                    Dictionary<EItems, HashSet<EItems>> tempRequiredItems = GetRequiredItemsFromMappings(mappings);

                    int logicalMappingAttempts = 1;
                    while (tempRequiredItems.Count > 0)
                    {
                        if (logicalMappingAttempts > REQUIRED_ITEM_PLACEMENT_ATTEMPT_LIMIT)
                        {
                            throw new RandomizerException($"Logical mapping attempts amount exceeded. Check to make sure there are no bugs causing potential infinite loops in seed '{seed}'");
                        }

                        //Send these items through the logical mapper and get them a home
                        LogicalMapping(tempRequiredItems, ref mappings);
                        //Repeat the required item gathering. We expect if all items were accounted for that we would receive an empty set
                        tempRequiredItems = GetRequiredItemsFromMappings(mappings);
                        ++logicalMappingAttempts;
                    }

                    //At this point we should be done with logical mapping. Let's cleanup the remaining items.
                    FastMapping(randomizedItems, ref mappings);
                    Console.WriteLine("Basic logic mapping completed.");
                    break;
            }
            //The mappings should be created now.
            return mappings;
        }


        public static int GenerateSeed()
        {
            int seed = (int)(DateTime.Now.Ticks & 0x0000DEAD);
            Console.WriteLine($"Seed '{seed}' generated."); 
            return seed;
        }

        public static bool IsSeedBeatable(int seed, Dictionary<SettingType, SettingValue> settings)
        {
            try
            {
                GenerateRandomizedMappings(new SeedRO(SeedType.Logic, seed, settings));
                return true;
            }
            catch(RandomizerException rde)
            {
                //This means that the seed was deemed not beatable
                Console.WriteLine($"Seed '{seed}' was deemed not beatable during IsSeedBeatable check. Error message received: '{rde.Message}'");
                return false;
            }
        }

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

        //Get the version number
        public static string GetModVersion()
        {
            string version = "Unknown";
            
            foreach(CourierModuleMetadata modMetadata in Courier.Mods)
            {
                if("TheMessengerRandomizer".Equals(modMetadata.Name))
                {
                    version = modMetadata.VersionString;
                }
            }

            return version;
        }

        private static void FastMapping(List<EItems> items, ref Dictionary<LocationRO, EItems> locationToItemMapping)
        {
            //Setting up local list to make sure of what I am messing with.
            List<EItems> localItems = new List<EItems>(items); 

            //randomly place passed items into available locations without checking logic requirements
            for (int itemIndex = randomNumberGen.Next(localItems.Count); localItems.Count > 0; itemIndex = randomNumberGen.Next(localItems.Count))
            {
                int locationIndex = randomNumberGen.Next(randomizedLocations.Count);
                Console.WriteLine($"Item Index '{itemIndex}' generated for item list with size '{localItems.Count}'. Locations index '{locationIndex}' generated for location list with size '{randomizedLocations.Count}'");
                locationToItemMapping.Add(randomizedLocations[locationIndex], localItems[itemIndex]);
                Console.WriteLine($"Fast mapping occurred. Added item '{localItems[itemIndex]}' at index '{itemIndex}' to check '{randomizedLocations[locationIndex].PrettyLocationName}' at index '{locationIndex}'.");
                //Removing mapped items and locations
                randomizedItems.Remove(localItems[itemIndex]); //Doing this just in case its in the main list
                Console.WriteLine($"Removing location at index '{locationIndex}' from location list sized '{randomizedLocations.Count}'");
                randomizedLocations.RemoveAt(locationIndex);
                Console.WriteLine($"Removing item at index '{itemIndex}' from items list sized '{localItems.Count}'");
                localItems.RemoveAt(itemIndex);
            }
            //All the passed items should now have a home
        }

        /// <summary>
        /// Will complete the mappings per item received. This mapping takes in to account the required items for each location it tries to place an item into to avoid basic lockouts. It relies on the tempRequiredItems map.
        /// </summary>
        private static void LogicalMapping(Dictionary<EItems, HashSet<EItems>> tempRequiredItems, ref Dictionary<LocationRO, EItems> locationToItemMapping)
        {
            //Creating local copy of required items so i know what I am messing with.
            Dictionary<EItems, HashSet<EItems>> localRequiredItems = new Dictionary<EItems, HashSet<EItems>>(tempRequiredItems);

            foreach (EItems item in localRequiredItems.Keys)
            {
                bool hasAHome = false;

                //create a new list based off the randomized locations list that has a randomized order. This will be used to placing things.
                List<LocationRO> tempRandoLocations = new List<LocationRO>(randomizedLocations);
                List<LocationRO> randoSortedLocations = new List<LocationRO>();
                //Populate new list
                for (int locationIndex = randomNumberGen.Next(tempRandoLocations.Count); tempRandoLocations.Count > 0; locationIndex = randomNumberGen.Next(tempRandoLocations.Count))
                {
                    randoSortedLocations.Add(tempRandoLocations[locationIndex]);
                    tempRandoLocations.RemoveAt(locationIndex);
                }

                //Find a home
                for (int i = 0; i < randoSortedLocations.Count; i++)
                {
                    hasAHome = IsLocationSafeForItem(randoSortedLocations[i], item);
                    //Check the item itself
                    if (hasAHome)
                    {
                        //Next we need to check the location for each and every item this item blocks. We need to catch the moment an item proves it cannot be here and mark it so we can move on.
                        foreach (EItems blockedItem in localRequiredItems[item])
                        {
                            hasAHome = IsLocationSafeForItem(randoSortedLocations[i], blockedItem);

                            if (!hasAHome)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        hasAHome = false;
                    }



                    if (hasAHome)
                    {
                        Console.WriteLine($"Found a home for item '{item}' at location '{randoSortedLocations[i].PrettyLocationName}'.");
                        locationToItemMapping.Add(randoSortedLocations[i], item);
                        randomizedLocations.Remove(randoSortedLocations[i]);
                        randomizedItems.Remove(item);
                        break;
                    }
                }
                if (!hasAHome)
                {
                    //Getting here means that we must have checked through all the remaining locations and that none of them could house an item we needed to place. For now let's throw an exception.
                    throw new RandomizerException("This seed was not completeable due to running out of locations to place things.");
                }
            }
        }

        private static bool IsLocationSafeForItem(LocationRO location, EItems item)
        {
            bool isSafe = false;

            switch (item)
            {
                case EItems.WINGSUIT: //Try to find a home for wingsuit.
                    if (!location.IsWingsuitRequired)
                    {
                        //if a coin flip on this location hasn't happened yet, do it now.
                        if (!coinResults.ContainsKey(location))
                        {
                            coinResults.Add(location, randomNumberGen.Next(2));
                        }
                        //if the location is not a RDorWS check we are good
                        //If it IS a RDorWS check, check to see if it is locked based on RD(coin flip is 1) If it is we are good. 
                        isSafe = !(location.IsEitherWingsuitOrRopeDartRequired && coinResults[location] != 1);
                    }
                    break;
                case EItems.GRAPLOU: //same for rope dart
                    if (!location.IsRopeDartRequired)
                    {
                        //if a coin flip on this location hasn't happened yet, do it now.
                        if (!coinResults.ContainsKey(location))
                        {
                            coinResults.Add(location, randomNumberGen.Next(2));
                        }

                        //if the location is not a RDorWS check we are good
                        //If it IS a RDorWS check, check to see if it is locked based on RD(coin flip is 1) If it is we are good. 
                        isSafe = !(location.IsEitherWingsuitOrRopeDartRequired && coinResults[location] == 1);
                    }
                    break;
                case EItems.MAGIC_BOOTS: //Tabis, the check is more simple
                    if (!location.IsNinjaTabiRequired)
                    {
                        isSafe = true;
                    }
                    break;
                default: //All other required items
                    if (!location.AdditionalRequiredItemsForCheck.Contains(item))
                    {
                        isSafe = true;
                    }
                    break;
            }
            Console.WriteLine($"Item '{item}' is safe at Location '{location.PrettyLocationName}' --- {isSafe}");
            return isSafe;
        }

        private static Dictionary<EItems, HashSet<EItems>> GetRequiredItemsFromMappings(Dictionary<LocationRO, EItems> mappings)
        {
            //Check through the current mappings and return a mapping of all required items and the items they are blocking. If the required item is already placed in a location, we will ignore it. 
            Dictionary<EItems, HashSet<EItems>> tempRequiredItems = new Dictionary<EItems, HashSet<EItems>>();
            //Key Items set so I can control how many of those I choose to handle per run
            HashSet<EItems> keyItems = new HashSet<EItems>();

            foreach (LocationRO location in mappings.Keys)
            {
                //Lets start interrogating the location object to see what items it has marked as required. Let's start with the key items.
                if (location.IsWingsuitRequired && randomizedItems.Contains(EItems.WINGSUIT))
                {
                    tempRequiredItems = AddRequiredItem(EItems.WINGSUIT, mappings[location], tempRequiredItems);
                    keyItems.Add(EItems.WINGSUIT);
                }
                if (location.IsRopeDartRequired && randomizedItems.Contains(EItems.GRAPLOU))
                {
                    tempRequiredItems = AddRequiredItem(EItems.GRAPLOU, mappings[location], tempRequiredItems);
                    keyItems.Add(EItems.GRAPLOU);
                }
                if (location.IsNinjaTabiRequired && randomizedItems.Contains(EItems.MAGIC_BOOTS))
                {
                    tempRequiredItems = AddRequiredItem(EItems.MAGIC_BOOTS, mappings[location], tempRequiredItems);
                    keyItems.Add(EItems.MAGIC_BOOTS);
                }
                //Checking if either Wingsuit OR Rope Dart is required is a separate check.
                if (location.IsEitherWingsuitOrRopeDartRequired)
                {
                    //In this case, let's randomly pick one to be placed somewhere
                    int coin;
                    if (coinResults.ContainsKey(location))
                    {
                        coin = coinResults[location];
                    }
                    else
                    {
                        coin = randomNumberGen.Next(2);
                        coinResults.Add(location, coin);
                    }

                    

                    switch (coin)
                    {
                        case 0://Wingsuit
                            if (randomizedItems.Contains(EItems.WINGSUIT))
                            {
                                tempRequiredItems = AddRequiredItem(EItems.WINGSUIT, mappings[location], tempRequiredItems);
                                keyItems.Add(EItems.WINGSUIT);
                            }
                            break;
                        case 1://Rope Dart
                            if (randomizedItems.Contains(EItems.GRAPLOU))
                            {
                                tempRequiredItems = AddRequiredItem(EItems.GRAPLOU, mappings[location], tempRequiredItems);
                                keyItems.Add(EItems.GRAPLOU);
                            }
                            break;
                        default://Something weird happened...just do wingsuit :P
                            if (randomizedItems.Contains(EItems.WINGSUIT))
                            {
                                tempRequiredItems = AddRequiredItem(EItems.WINGSUIT, mappings[location], tempRequiredItems);
                                keyItems.Add(EItems.WINGSUIT);
                            }
                            break;
                    }
                }

                //Next lets look through the other items. 
                foreach (EItems requiredItem in location.AdditionalRequiredItemsForCheck)
                {

                    if (EItems.NONE != requiredItem && randomizedItems.Contains(requiredItem))
                    {
                        tempRequiredItems = AddRequiredItem(requiredItem, mappings[location], tempRequiredItems);
                    }
                }
            }

            //I was having a problem with some seeds setting all the key items at the beginning and not considering each other. I think how I will handle this is by only allowing one of them set each run through and throwing the rest out. I expect them to get picked up on subsequent runs.
            if (keyItems.Count > 1)
            {
                //This means I have more than 1 key item to process. Let's pick random ones to remove from the required items list until none remain.
                for (int i = randomNumberGen.Next(keyItems.Count); keyItems.Count > 1; i = randomNumberGen.Next(keyItems.Count))
                {
                    EItems itemToRemove = keyItems.ElementAt(i);
                    Console.WriteLine($"Found multiple key items during required item mapping. Tossing '{itemToRemove}' from this run.");
                    tempRequiredItems.Remove(itemToRemove);
                    keyItems.Remove(itemToRemove);
                }
            }

            //Logging
            Console.WriteLine("For the provided checks: ");
            foreach (LocationRO location in mappings.Keys)
            {
                Console.WriteLine(location.PrettyLocationName);
            }
            Console.WriteLine("Found these items to require for seed:");
            foreach (EItems requiredItem in tempRequiredItems.Keys)
            {
                Console.WriteLine(requiredItem);
                foreach (EItems blockedItem in tempRequiredItems[requiredItem])
                {
                    Console.WriteLine($"\tWhich in turn blocks '{blockedItem}'");
                }
            }
            if (tempRequiredItems.Count == 0)
            {
                Console.WriteLine("No required items found, returning an empty set!");
            }
            Console.WriteLine("Required item determination complete!");
            //Logging complete
            //All done!
            return tempRequiredItems;
        }

        private static Dictionary<EItems, HashSet<EItems>> AddRequiredItem(EItems item, EItems blockedItem, Dictionary<EItems, HashSet<EItems>> tempRequiredItems)
        {
            //This utility function will help manage the temporary required item dictionary for me.

            //Check to see if the item is already a key in the dictionary. If not, add it.
            if (!tempRequiredItems.ContainsKey(item))
            {
                tempRequiredItems.Add(item, new HashSet<EItems>());
                //Perform archival of blockers as needed
                if (!requiredItemsWithBlockers.ContainsKey(item))
                {
                    requiredItemsWithBlockers.Add(item, new HashSet<EItems>());
                }
            }
            //Add the blocked item to this item's set
            if (!RandomizerConstants.notes.Contains(blockedItem)) //dont care about notes
            {
                tempRequiredItems[item].Add(blockedItem);
                requiredItemsWithBlockers[item].Add(blockedItem);
            }

            //Check through archival and add those to this blocker list as well.
            if (requiredItemsWithBlockers.ContainsKey(blockedItem))
            {
                foreach (EItems nestedBlockedItem in requiredItemsWithBlockers[blockedItem])
                {
                    tempRequiredItems[item].Add(nestedBlockedItem);
                    requiredItemsWithBlockers[item].Add(blockedItem);
                }
            }

            return tempRequiredItems;
        }
    }
}
