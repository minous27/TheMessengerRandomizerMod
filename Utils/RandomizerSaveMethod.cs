using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerRando.RO;
using MessengerRando.Utils;
using Mod.Courier.Save;

namespace MessengerRando
{
    //Format for the mod save value: |seed1+seedType&seedOption1=seedOption1Value&seedOption2=seedOption2Value|seed2+seedType&seedOption1=seedOption1Value&seedOption2=seedOption2Value|seed3+seedType&seedOption1=seedOption1Value&seedOption2=seedOption2Value
    class RandomizerSaveMethod : OptionSaveMethod
    {
        private const char RANDO_OPTION_VALUE_DELIM = '|';
        private const char RANDO_OPTION_TYPE_DELIM = '+';
        private const char RANDO_OPTION_SETTING_DELIM = '&';
        private const char RANDO_OPTION_SETTING_VALUE_DELIM = '=';

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
                SeedRO seed = stateManager.GetSeedForFileSlot(i);
                modValue.Append("" + RANDO_OPTION_VALUE_DELIM + seed.Seed + RANDO_OPTION_TYPE_DELIM + seed.SeedType);

                foreach(SettingType setting in seed.Settings.Keys)
                {
                    modValue.Append("" + RANDO_OPTION_SETTING_DELIM + setting + RANDO_OPTION_SETTING_VALUE_DELIM + seed.Settings[setting]);
                }
            }

            Console.WriteLine($"Saving seed data: '{modValue}'");

            return modValue.ToString();
        }

        public override void Load(string load)
        {
            Console.WriteLine($"Received value during mod option load: '{load}'");
            //Split on delimeter to get all seeds
            string[] seeds = load.Split(RANDO_OPTION_VALUE_DELIM);
            Console.WriteLine("load data split into seeds");
            for(int i = 1; i < seeds.Length; i++)
            {
                string seedDetails = seeds[i];
                Console.WriteLine($"Adding '{seedDetails}' to state manager.");

                //find necessary indicies in the string
                int randoTypeIndex = seedDetails.IndexOf(RANDO_OPTION_TYPE_DELIM);
                int randoSettingIndex = seedDetails.IndexOf(RANDO_OPTION_SETTING_DELIM);

                string seedSub = seedDetails.Substring(0, randoTypeIndex);
                Console.WriteLine($"Extracted seed '{seedSub}' from seed split {i}");
                //This will parse the seed into an int. If the value cannot be parsed for some reason, seed will be 0
                Int32.TryParse(seedSub, out int seed);

                //Need to check if there are settings for this seed. If so, consider them when getting the seedtype. If not, the rest of the string is the seedtype.
                string seedTypeSub = randoSettingIndex != -1 ? seedDetails.Substring(randoTypeIndex + 1, randoSettingIndex - (randoTypeIndex + 1)) : seedDetails.Substring(randoTypeIndex + 1);
                Console.WriteLine($"Extracted seedtype '{seedTypeSub}' from seed split {i}");
                //This will pull out the seed type. If there is none, default it.
                SeedType seedType = SeedType.None;
                if (seedTypeSub != null && Enum.IsDefined(typeof(SeedType), seedTypeSub)) //using IsDefined because I dont have TryParse in this .NET version T_T
                {
                    seedType = (SeedType)Enum.Parse(typeof(SeedType), seedTypeSub);
                }
                //If there are settings, I need to pull them out as well
                Dictionary<SettingType, SettingValue> seedSettings = new Dictionary<SettingType, SettingValue>();

                if(randoSettingIndex != -1)
                {
                    string seedSettingSub = seedDetails.Substring(seedDetails.IndexOf(RANDO_OPTION_SETTING_DELIM) + 1);
                    Console.WriteLine($"Extracted seed settings '{seedSettingSub}' from seed split {i}");

                    string[] splitSeedSettings = seedSettingSub.Split(RANDO_OPTION_SETTING_DELIM);
                    
                    foreach(string setting in splitSeedSettings)
                    {
                        string[] splitSeedSetting = setting.Split(RANDO_OPTION_SETTING_VALUE_DELIM);
                        string splitSeedSettingType = splitSeedSetting[0];
                        string splitSeedSettingValue = splitSeedSetting[1];

                        Console.WriteLine($"Split setting '{splitSeedSettingType}' with value '{splitSeedSettingValue}' added to seed split {i}");
                        
                        if((splitSeedSettingType != null && Enum.IsDefined(typeof(SettingType), splitSeedSettingType)) && (splitSeedSettingValue != null && Enum.IsDefined(typeof(SettingValue), splitSeedSettingValue)))
                        {
                            seedSettings.Add((SettingType)Enum.Parse(typeof(SettingType), splitSeedSettingType), (SettingValue)Enum.Parse(typeof(SettingValue), splitSeedSettingValue));
                        }

                        
                    }

                }

                stateManager.AddSeed(i, seedType, seed, seedSettings);
                Console.WriteLine($"'{seeds[i]}' added to state manager successfully.");
            }

            Console.WriteLine("Loading into state manager complete.");
        }
    }
}
