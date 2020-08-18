using System;
using System.Collections.Generic;
using System.Threading;
using Mod.Courier;
using Mod.Courier.Module;


namespace MessengerRando
{
    //This class will be responsible for handling the randomization of items to locations and generating the mapping dictionary.
    public class ItemRandomizerUtil
    {

        public static List<EItems> RandomizableItems { get; private set; }
        public static List<EItems> RandomizableLocations { get; private set; }
        public static List<string> TriggersToIgnoreRandoItemLogic { get; private set; }

        public static Dictionary<string, EItems> CutsceneMappings { get; private set; }

        public static int OfficialSeed { get; private set; }
        public static Random randomNumberGen;

        public static bool isEasyGrapple = false;
        public static bool arePricesRandomized = false;
        public static Dictionary<EShopUpgradeID,int> randomUpgradeData { get; private set; }


        public static Dictionary<EShopUpgradeID, int> GenerateRandomizedUpgradeMappings(int passedSeed = Int32.MinValue)
        {
            Dictionary<EShopUpgradeID, int> temp = new Dictionary<EShopUpgradeID, int>();
            foreach(KeyValuePair<EShopUpgradeID,int> KVP in randomUpgradeData)
            {
                int currentPrice = KVP.Value;
                int lowPrice = currentPrice / 2;
                int highPrice = currentPrice + lowPrice;
                int newPrice = randomNumberGen.Next(lowPrice, highPrice);
                Console.WriteLine($"We found { KVP.Key} with price {currentPrice} modified price is {newPrice}");
                temp.Add(KVP.Key, newPrice);
            }
            randomUpgradeData = temp;
            return randomUpgradeData;
        }

        public static Dictionary<EItems,EItems> GenerateRandomizedMappings(int passedSeed = Int32.MinValue)
        {

            Console.WriteLine($"Beginning mapping generation for seed '{OfficialSeed}'.");
            //We now have a seed. Lets also create a local copy of the locations so I can mess with it without breaking stuff.
            List<EItems> locationsForGeneration = new List<EItems>(RandomizableLocations);

            //Get our randomizer set up
            

            List<EItems> safeLocation = new List<EItems>();
            safeLocation.Add(EItems.WINGSUIT);
            safeLocation.Add(EItems.MAGIC_BOOTS);
            safeLocation.Add(EItems.SEASHELL);
            //Begin filling out the mappings. Both collections need to logically be the same size.
            Dictionary<EItems, EItems> mappings = new Dictionary<EItems, EItems>();
            foreach (EItems item in RandomizableItems) //For each item to randomize, pick a random location and create the mapping.
            {
                //Make sure the Grapple is found in two early accessable locations - Toggle in menu
                if (isEasyGrapple && item == EItems.GRAPLOU) 
                {
                    int index = randomNumberGen.Next(safeLocation.Count);
                    mappings.Add(safeLocation[index], item);
                    Console.WriteLine($"Mapping added! '{item}' can be found at '{safeLocation[index]}'");
                    locationsForGeneration.Remove(safeLocation[index]);
                    safeLocation.RemoveAt(index);
                    
                }else if(item == EItems.MAGIC_BOOTS) //Makes sure the Lightfoot Tabi is not stuck in a location which requires them.
                {
                    int index = randomNumberGen.Next(locationsForGeneration.Count);
                    EItems loc = locationsForGeneration[index];
                    while(loc == EItems.SUN_CREST || loc == EItems.MOON_CREST || loc == EItems.KEY_OF_CHAOS || loc == EItems.KEY_OF_LOVE || loc == EItems.PYROPHOBIC_WORKER)
                    {
                        Console.WriteLine($"Tried to map {item} to {loc} would result in softlock... remapping...");
                        index = randomNumberGen.Next(locationsForGeneration.Count);
                        loc = locationsForGeneration[index];
                    }
                    mappings.Add(locationsForGeneration[index], item);
                    Console.WriteLine($"Mapping added! '{item}' can be found at '{locationsForGeneration[index]}'");
                    locationsForGeneration.RemoveAt(index);

                }else if(item == EItems.SUN_CREST || item == EItems.MOON_CREST) //Makes sure the Sun and Moon crest can not be behind the door
                {
                    int index = randomNumberGen.Next(locationsForGeneration.Count);
                    EItems loc = locationsForGeneration[index];
                    while ( loc == EItems.KEY_OF_LOVE)
                    {
                        Console.WriteLine($"Tried to map {item} to {loc} would result in softlock... remapping...");
                        index = randomNumberGen.Next(locationsForGeneration.Count);
                        loc = locationsForGeneration[index];
                    }
                    mappings.Add(locationsForGeneration[index], item);
                    Console.WriteLine($"Mapping added! '{item}' can be found at '{locationsForGeneration[index]}'");
                    locationsForGeneration.RemoveAt(index);
                }
                else
                {
                    int index = randomNumberGen.Next(locationsForGeneration.Count);
                    mappings.Add(locationsForGeneration[index], item);
                    Console.WriteLine($"Mapping added! '{item}' can be found at '{locationsForGeneration[index]}'");
                    //Remove the used location
                    locationsForGeneration.RemoveAt(index);
                }
            }

            //The mappings should be created now.
            Console.WriteLine("Mapping generation complete.");
            return mappings;
        }

        public static void Load()
        {
            LoadRandomizableItems();
            LoadRandomizableLocations();
            LoadRandomizableUpgrades();
            LoadSpecialTriggerNames();
            LoadCutsceneMappings();
        }

        public static int GenerateSeed()
        {
            int seed = (int)(DateTime.Now.Ticks & 0x0000DEAD);
            Console.WriteLine($"Seed '{seed}' generated.");
         //   GenerateRandomizedMappings(seed); //Right now mostly generating here to log mappings before entering game. 
            return seed;
        }

        public static int getSeed(int passedSeed)
        {
            //If no seed was provided, create one
            if (passedSeed == Int32.MinValue)
            {
                int tempSeed = GenerateSeed();

                if (OfficialSeed == tempSeed)
                {
                    Console.WriteLine("Generated the same seed again. Will wait a bit and try again.");
                    Thread.Sleep(1000);//Wait a second
                    tempSeed = GenerateSeed();
                    //Do the check one more time and if nothing was fixed then log it.
                    if (OfficialSeed == tempSeed)
                    {
                        Console.WriteLine("2 attempts to get a new seed failed. Moving along...");
                    }
                }

                OfficialSeed = tempSeed;


                Console.WriteLine($"No seed passed, generated seed for this mapping is: {OfficialSeed}");
            }
            else
            {
                OfficialSeed = passedSeed;
            }

            return OfficialSeed;
        }
        private static void LoadRandomizableUpgrades()
        {
            Dictionary<EShopUpgradeID, int> newUpgradeData = new Dictionary<EShopUpgradeID, int>();
            newUpgradeData.Add(EShopUpgradeID.AIR_RECOVER,80);
            newUpgradeData.Add(EShopUpgradeID.ATTACK_PROJECTILE,40);
            newUpgradeData.Add(EShopUpgradeID.CHARGED_ATTACK,2000);
            newUpgradeData.Add(EShopUpgradeID.CHECKPOINT_FULL,550);
            newUpgradeData.Add(EShopUpgradeID.DAMAGE_REDUCTION,1000);
            newUpgradeData.Add(EShopUpgradeID.ENEMY_DROP_HP,200);
            newUpgradeData.Add(EShopUpgradeID.ENEMY_DROP_MANA,200);
            newUpgradeData.Add(EShopUpgradeID.GLIDE_ATTACK,150);
            newUpgradeData.Add(EShopUpgradeID.HP_UPGRADE_1,30);
            newUpgradeData.Add(EShopUpgradeID.HP_UPGRADE_2,320);
            newUpgradeData.Add(EShopUpgradeID.POTION_FULL_HEAL_AND_HP,350);
            newUpgradeData.Add(EShopUpgradeID.POWER_SEAL,250);
            newUpgradeData.Add(EShopUpgradeID.POWER_SEAL_WORLD_MAP,250);
            newUpgradeData.Add(EShopUpgradeID.QUARBLE_DISCOUNT_50,400);
            newUpgradeData.Add(EShopUpgradeID.SHURIKEN,50);
            newUpgradeData.Add(EShopUpgradeID.SHURIKEN_UPGRADE_1,250);
            newUpgradeData.Add(EShopUpgradeID.SHURIKEN_UPGRADE_2,350);
            newUpgradeData.Add(EShopUpgradeID.SWIM_DASH,125);
            newUpgradeData.Add(EShopUpgradeID.TIME_WARP,250);
            randomUpgradeData = newUpgradeData;

           // randomUpgradeData = listofUpgrades;
        }
        private static void LoadRandomizableItems()
        {
            List<EItems> itemsToLoad = new List<EItems>();
            itemsToLoad.Add(EItems.GRAPLOU);
            itemsToLoad.Add(EItems.MAGIC_BOOTS);
            itemsToLoad.Add(EItems.MOON_CREST);
            itemsToLoad.Add(EItems.SUN_CREST);



            itemsToLoad.Add(EItems.WINGSUIT);
            
            itemsToLoad.Add(EItems.WINDMILL_SHURIKEN);
            /*Making elder quest chain vanilla for now. Need to handle it's complex checks before i rando it.
            itemsToLoad.Add(EItems.TEA_SEED);
            */

            itemsToLoad.Add(EItems.POWER_THISTLE);
            itemsToLoad.Add(EItems.FAIRY_BOTTLE);
            
            
            itemsToLoad.Add(EItems.RUXXTIN_AMULET);
            itemsToLoad.Add(EItems.DEMON_KING_CROWN);

            itemsToLoad.Add(EItems.CANDLE);
            itemsToLoad.Add(EItems.SEASHELL);

            itemsToLoad.Add(EItems.NECROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.CLAUSTROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.PYROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.ACROPHOBIC_WORKER);

            itemsToLoad.Add(EItems.KEY_OF_CHAOS);
            itemsToLoad.Add(EItems.KEY_OF_COURAGE);
            itemsToLoad.Add(EItems.KEY_OF_HOPE);
            itemsToLoad.Add(EItems.KEY_OF_LOVE);
            itemsToLoad.Add(EItems.KEY_OF_STRENGTH);
            itemsToLoad.Add(EItems.KEY_OF_SYMBIOSIS);

            RandomizableItems = itemsToLoad;
        }

        private static void LoadRandomizableLocations()
        {
            List<EItems> locationsToLoad = new List<EItems>();
            
            locationsToLoad.Add(EItems.WINGSUIT);
            locationsToLoad.Add(EItems.GRAPLOU);
            locationsToLoad.Add(EItems.MAGIC_BOOTS);
            locationsToLoad.Add(EItems.CLIMBING_CLAWS);

            locationsToLoad.Add(EItems.POWER_THISTLE);
            locationsToLoad.Add(EItems.FAIRY_BOTTLE);
            locationsToLoad.Add(EItems.SUN_CREST);
            locationsToLoad.Add(EItems.MOON_CREST);
            locationsToLoad.Add(EItems.RUXXTIN_AMULET);
            locationsToLoad.Add(EItems.DEMON_KING_CROWN);

            locationsToLoad.Add(EItems.CANDLE);
            locationsToLoad.Add(EItems.SEASHELL);

            locationsToLoad.Add(EItems.NECROPHOBIC_WORKER);
            locationsToLoad.Add(EItems.CLAUSTROPHOBIC_WORKER);
            locationsToLoad.Add(EItems.PYROPHOBIC_WORKER);
            locationsToLoad.Add(EItems.ACROPHOBIC_WORKER);

            locationsToLoad.Add(EItems.KEY_OF_CHAOS);
            locationsToLoad.Add(EItems.KEY_OF_COURAGE);
            locationsToLoad.Add(EItems.KEY_OF_HOPE);
            locationsToLoad.Add(EItems.KEY_OF_LOVE);
            locationsToLoad.Add(EItems.KEY_OF_STRENGTH);
            locationsToLoad.Add(EItems.KEY_OF_SYMBIOSIS);

            RandomizableLocations = locationsToLoad;
        }

        private static void LoadSpecialTriggerNames()
        {
            TriggersToIgnoreRandoItemLogic = new List<string>();

            //LOAD (initally started as a black list of locations...probably would have been better to make this a whitelist...whatever)
            TriggersToIgnoreRandoItemLogic.Add("CorruptedFuturePortal"); //Need to really check for crown and get access to CF
            TriggersToIgnoreRandoItemLogic.Add("Lucioles"); //CF Fairy Check
            TriggersToIgnoreRandoItemLogic.Add("DecurseQueenCutscene");
            TriggersToIgnoreRandoItemLogic.Add("Bridge"); //Forlorn bridge check
            TriggersToIgnoreRandoItemLogic.Add("NoUpgrade"); //Dark Cave Candle check
            TriggersToIgnoreRandoItemLogic.Add("OverlayArt_16"); //...also Dark Cave Candle check
            //These are for the sprite renderings of phoebes
            TriggersToIgnoreRandoItemLogic.Add("PhobekinNecro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinNecro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinAcro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinAcro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinClaustro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinClaustro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinPyro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinPyro_16");
            //Parents of triggers to handle sassy interaction zones
            TriggersToIgnoreRandoItemLogic.Add("Colos_8");
            TriggersToIgnoreRandoItemLogic.Add("Suses_8");
            TriggersToIgnoreRandoItemLogic.Add("Door");
            TriggersToIgnoreRandoItemLogic.Add("RuxtinStaff");
        }

        private static void LoadCutsceneMappings()
        {
            //This is where all the cutscene mappings will live. These mappings will mean that the cutscene requires additional logic to ensure it has "been played" or not.
            CutsceneMappings = new Dictionary<string, EItems>();

            //LOAD
            CutsceneMappings.Add("RuxxtinNoteAndAwardAmuletCutscene", EItems.RUXXTIN_AMULET);

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
    }
}
