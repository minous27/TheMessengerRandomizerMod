using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerRando
{
    //This class will be responsible for handling the randomization of items to locations and generating the mapping dictionary.
    public class ItemRandomizerUtil
    {

        public static List<EItems> RandomizableItems { get; private set; }
        public static List<EItems> RandomizableLocations { get; private set; }


        public static Dictionary<EItems,EItems> GenerateRandomizedMappings()
        {
            Console.WriteLine("Beginning mapping generation.");
            Random randomNumberGen = new Random();
            Dictionary<EItems, EItems> mappings = new Dictionary<EItems, EItems>();
            //Begin filling out the mappings. Both collections need to logically be the same size.
            foreach (EItems item in RandomizableItems) //For each item to randomize, pick a random location and create the mapping.
            {
                int index = randomNumberGen.Next(RandomizableLocations.Count);
                mappings.Add(RandomizableLocations[index], item);
                Console.WriteLine($"Mapping added! '{item}' can be found at '{RandomizableLocations[index]}'");
                //Remove the used location
                RandomizableLocations.RemoveAt(index);
            }
            //The mappings should be created now.
            Console.WriteLine("Mapping generation complete.");
            return mappings;
        }

        public static void Load()
        {
            LoadRandomizableItems();
        }


        private static void LoadRandomizableItems()
        {
            List<EItems> itemsToLoad = new List<EItems>();
            itemsToLoad.Add(EItems.CLIMBING_CLAWS);
            itemsToLoad.Add(EItems.WINGSUIT);
            itemsToLoad.Add(EItems.GRAPLOU);
            itemsToLoad.Add(EItems.SEASHELL);
            itemsToLoad.Add(EItems.TEA_SEED);
            itemsToLoad.Add(EItems.CANDLE);
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
    }
}
