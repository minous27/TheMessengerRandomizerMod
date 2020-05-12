using System;
using System.Collections.Generic;
using System.Threading;


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

        public static Dictionary<EItems,EItems> GenerateRandomizedMappings(int passedSeed = Int32.MinValue)
        {
            Console.WriteLine("Beginning mapping generation.");
            //If no seed was provided, create one
            if(passedSeed == Int32.MinValue)
            {
                int tempSeed = GenerateSeed();

                if (OfficialSeed == tempSeed)
                {
                    Console.WriteLine("Generated the same seed again. Will wait a bit and try again.");
                    Thread.Sleep(1000);//Wait a second
                    tempSeed = GenerateSeed();
                    //Do the check one more time and if nothing was fixed then log it.
                    if(OfficialSeed == tempSeed)
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
            //We now have a seed. Lets also create a local copy of the locations so I can mess with it without breaking stuff.
            List<EItems> locationsForGeneration = new List<EItems>(RandomizableLocations);

            //Get our randomizer set up
            Random randomNumberGen = new Random(OfficialSeed);

            //Begin filling out the mappings. Both collections need to logically be the same size.
            Dictionary<EItems, EItems> mappings = new Dictionary<EItems, EItems>();
            foreach (EItems item in RandomizableItems) //For each item to randomize, pick a random location and create the mapping.
            {
                int index = randomNumberGen.Next(locationsForGeneration.Count);
                mappings.Add(locationsForGeneration[index], item);
                Console.WriteLine($"Mapping added! '{item}' can be found at '{locationsForGeneration[index]}'");
                //Remove the used location
                locationsForGeneration.RemoveAt(index);
            }
            //The mappings should be created now.
            Console.WriteLine("Mapping generation complete.");
            return mappings;
        }

        public static void Load()
        {
            LoadRandomizableItems();
            LoadSpecialTriggerNames();
            //LoadCutsceneMappings();
        }

        public static int GenerateSeed()
        {
            int seed = (int)(DateTime.Now.Ticks & 0x0000DEAD);
            Console.WriteLine($"Seed '{seed}' generated.");
            GenerateRandomizedMappings(seed); //Right now mostly generating here to log mappings before entering game. 
            return seed;
        }


        private static void LoadRandomizableItems()
        {
            List<EItems> itemsToLoad = new List<EItems>();
            itemsToLoad.Add(EItems.WINGSUIT);
            itemsToLoad.Add(EItems.GRAPLOU);
            itemsToLoad.Add(EItems.SEASHELL);
            /*Making elder quest chain vanilla for now. Need to handle it's complex checks before i rando it.
            itemsToLoad.Add(EItems.TEA_SEED);
            itemsToLoad.Add(EItems.CANDLE);
            */
            itemsToLoad.Add(EItems.POWER_THISTLE);
            itemsToLoad.Add(EItems.FAIRY_BOTTLE);
            itemsToLoad.Add(EItems.SUN_CREST);
            itemsToLoad.Add(EItems.MOON_CREST);
            itemsToLoad.Add(EItems.MAGIC_BOOTS);
            itemsToLoad.Add(EItems.RUXXTIN_AMULET);
            itemsToLoad.Add(EItems.NECROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.CLAUSTROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.PYROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.ACROPHOBIC_WORKER);
            itemsToLoad.Add(EItems.DEMON_KING_CROWN);
            itemsToLoad.Add(EItems.KEY_OF_CHAOS);
            itemsToLoad.Add(EItems.KEY_OF_COURAGE);
            itemsToLoad.Add(EItems.KEY_OF_HOPE);
            itemsToLoad.Add(EItems.KEY_OF_LOVE);
            itemsToLoad.Add(EItems.KEY_OF_STRENGTH);
            itemsToLoad.Add(EItems.KEY_OF_SYMBIOSIS);

            RandomizableItems = itemsToLoad;
            //For now the lists will be the same so lets set the locations as well.
            RandomizableLocations = new List<EItems>(itemsToLoad);

        }

        private static void LoadSpecialTriggerNames()
        {
            TriggersToIgnoreRandoItemLogic = new List<string>();

            //LOAD (initally started as a black list of locations...probably would have been better to make this a whitelist...whatever)
            TriggersToIgnoreRandoItemLogic.Add("CorruptedFuturePortal"); //Need to really check for crown and get access to CF
            TriggersToIgnoreRandoItemLogic.Add("Lucioles"); //CF Fairy Check
            TriggersToIgnoreRandoItemLogic.Add("InteractionZone"); //Sunken Shrine door check...also the name of the power thistle give check
            TriggersToIgnoreRandoItemLogic.Add("DecurseQueenCutscene");
            TriggersToIgnoreRandoItemLogic.Add("Bridge"); //Forlorn bridge check
            //These are for the sprite renderings of phoebes
            TriggersToIgnoreRandoItemLogic.Add("PhobekinNecro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinNecro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinAcro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinAcro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinClaustro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinClaustro_16");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinPyro");
            TriggersToIgnoreRandoItemLogic.Add("PhobekinPyro_16");
        }

        /*Currently not manipulating cutscenes specifically but leaving the code here because I am sure i will at some point.
        private static void LoadCutsceneMappings()
        {
            //This is where all the cutscene mappings will live. I will map them to items the player should have to indicate this cutscene should have "been played"
            CutsceneMappings = new Dictionary<string, EItems>();

            //LOAD
            CutsceneMappings.Add("CorruptedFuturePortalOpeningCutscene", EItems.DEMON_KING_CROWN);

        }
        */
    }
}
