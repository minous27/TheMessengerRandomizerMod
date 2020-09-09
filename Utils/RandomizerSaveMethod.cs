using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerRando.Utils;
using Mod.Courier.Save;

namespace MessengerRando
{
    //Format for the mod save value: |seed1+seedType|seed2+seedType|seed3+seedType
    class RandomizerSaveMethod : OptionSaveMethod
    {
        private const string RANDO_OPTION_VALUE_DELIM = "|";
        private const string RANDO_OPTION_TYPE_DELIM = "+";

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
                modValue.Append(RANDO_OPTION_VALUE_DELIM + stateManager.GetSeedForFileSlot(i).Seed + RANDO_OPTION_TYPE_DELIM + stateManager.GetSeedForFileSlot(i).SeedType);
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
                string[] seedDetails = seeds[i].Split(RANDO_OPTION_TYPE_DELIM.ToCharArray()); //expecting (0)seed - (1)seedType


                stateManager.AddSeed(i, (SeedType)Enum.Parse(typeof(SeedType),seedDetails[1], true), Int32.Parse(seedDetails[0]));
                Console.WriteLine($"'{seeds[i]}' added to state manager successfully.");
            }

            Console.WriteLine("Loading into state manager complete.");
        }
    }
}
