using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mod.Courier.Save;

namespace MessengerRando
{
    //Format for the mod save value: |seed1|seed2|seed3
    class RandomizerSaveMethod : OptionSaveMethod
    {
        private const string RANDO_OPTION_VALUE_DELIM = "|";

        private RandomizerStateManager stateManager;

        public RandomizerSaveMethod(string optionKey)
        {
            this.optionKey = optionKey;
            this.stateManager = RandomizerStateManager.Instance;
        }

        public override string Save()
        {
            StringBuilder modValue = new StringBuilder();
            
            for(int i = 1; i <= 3; i++)
            {
                modValue.Append(RANDO_OPTION_VALUE_DELIM + stateManager.GetSeedForFileSlot(i));
            }

            Console.WriteLine($"Saving seed data: '{modValue}'");

            return modValue.ToString();
        }

        public override void Load(string load)
        {
            Console.WriteLine($"Received value during mod option load: '{load}'");
            //Split on delimeter to get all seeds
            string[] seeds = load.Split(RANDO_OPTION_VALUE_DELIM.ToCharArray());
            Console.WriteLine("load data split into seeds");
            for(int i = 1; i < seeds.Length; i++)
            {
                Console.WriteLine($"Adding '{seeds[i]}' to state manager.");
                //add seeds to the state manager
                stateManager.AddSeed(i, Int32.Parse(seeds[i]));
                Console.WriteLine($"'{seeds[i]}' added to state manager successfully.");
            }

            Console.WriteLine("Loading into state manager complete.");
        }
    }
}
