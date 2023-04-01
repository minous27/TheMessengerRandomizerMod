using System;
using System.Collections.Generic;
using System.Text;
using MessengerRando.RO;
using MessengerRando.Utils;
using Mod.Courier;

namespace MessengerRando
{

    //Format for the mod save value: |{seed#}+{isLogicalSeed}&{Settings}&CollectedItems={collectedItems}&Mappings={mappingString}
    public class RandomizerSaveMethod 
    {
        private const char RANDO_OPTION_VALUE_DELIM = '|';
        private const char RANDO_OPTION_TYPE_DELIM = '+';
        private const char RANDO_OPTION_SETTING_DELIM = '&';
        private const char RANDO_OPTION_SETTING_VALUE_DELIM = '=';
        private const char RANDO_OPTION_ITEM_DELIM = ',';

        private RandomizerStateManager stateManager;

        public RandomizerSaveMethod()
        {
            this.stateManager = RandomizerStateManager.Instance;
        }

        public string GenerateSaveData()
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
                //Capturing collected items per seed
                if (seed.CollectedItems.Count > 0)
                {
                    modValue.Append("" + RANDO_OPTION_SETTING_DELIM + "CollectedItems=");
                    foreach (RandoItemRO collectedItem in seed.CollectedItems)
                    {
                        modValue.Append("" + collectedItem + RANDO_OPTION_ITEM_DELIM);
                    }
                    //Shaving off the last ','
                    modValue.Length--;
                }
                //Add seed info
                if (seed.MappingInfo != null && !seed.MappingInfo.Equals(""))
                {
                    modValue.Append("" + RANDO_OPTION_SETTING_DELIM + "Mappings=" + seed.MappingInfo);
                }
            }

            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Saving seed data: '{modValue}'");

            return modValue.ToString();
        }

        public void Load(string load)
        {
            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Received value during mod option load: '{load}'");
            //Split on delimeter to get all seeds
            string[] seeds = load.Split(RANDO_OPTION_VALUE_DELIM);
            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "load data split into seeds");
            for(int i = 1; i < seeds.Length; i++)
            {
                string seedDetails = seeds[i];
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Adding '{seedDetails}' to state manager.");

                //find necessary indicies in the string
                int randoTypeIndex = seedDetails.IndexOf(RANDO_OPTION_TYPE_DELIM);
                int randoSettingIndex = seedDetails.IndexOf(RANDO_OPTION_SETTING_DELIM);

                string seedSub = seedDetails.Substring(0, randoTypeIndex);
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Extracted seed '{seedSub}' from seed split {i}");
                
                //This will parse the seed into an int. If the value cannot be parsed for some reason, seed will be 0
                Int32.TryParse(seedSub, out int seed);

                //Need to check if there are settings for this seed. If so, consider them when getting the seedtype. If not, the rest of the string is the seedtype.
                string seedTypeSub = randoSettingIndex != -1 ? seedDetails.Substring(randoTypeIndex + 1, randoSettingIndex - (randoTypeIndex + 1)) : seedDetails.Substring(randoTypeIndex + 1);
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Extracted seedtype '{seedTypeSub}' from seed split {i}");
                
                //This will pull out the seed type. If there is none, default it.
                SeedType seedType = SeedType.None;
                if (seedTypeSub != null && Enum.IsDefined(typeof(SeedType), seedTypeSub)) //using IsDefined because I dont have TryParse in this .NET version T_T
                {
                    seedType = (SeedType)Enum.Parse(typeof(SeedType), seedTypeSub);
                }
                
                //If there are settings, I need to pull them out as well
                Dictionary<SettingType, SettingValue> seedSettings = new Dictionary<SettingType, SettingValue>();
                List<RandoItemRO> collectedRandoItemList = new List<RandoItemRO>();
                string mappingString = "";

                if (randoSettingIndex != -1)
                {
                    string seedSettingSub = seedDetails.Substring(seedDetails.IndexOf(RANDO_OPTION_SETTING_DELIM) + 1);
                    CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Extracted seed settings '{seedSettingSub}' from seed split {i}");

                    string[] splitSeedSettings = seedSettingSub.Split(RANDO_OPTION_SETTING_DELIM);
                    
                    foreach(string setting in splitSeedSettings)
                    {
                        //handle collected item loading
                        if (setting.StartsWith("CollectedItems="))
                        {
                            string collectedItemsRaw = setting.Substring(setting.IndexOf(RANDO_OPTION_SETTING_VALUE_DELIM) + 1);
                            //split further to get each rando item
                            string[] collectedItems = collectedItemsRaw.Split(RANDO_OPTION_ITEM_DELIM);
                            SaveGameSlot save = Manager<SaveManager>.Instance.GetSaveSlot(i - 1);
                            foreach (string collectedItem in collectedItems)
                            {
                                try
                                {
                                    RandoItemRO randoItemFromModSave = RandoItemRO.ParseString(collectedItem);

                                    if(save.Items.ContainsKey(randoItemFromModSave.Item))
                                    {
                                        collectedRandoItemList.Add(randoItemFromModSave);
                                        CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Added item '{collectedItem}' to Collected Item pool for file slot '{i}'.");
                                    }
                                    else
                                    {
                                        CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Item '{randoItemFromModSave.Item}' was not in the game save so we are ignoring it.");
                                    }
                                }
                                catch (Exception)
                                {
                                    CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"ERROR WHILE LOADING FROM MOD SAVE FILE: Found an item that could not be processed. Item in question '{collectedItem}'. Skipping item.");
                                    continue;
                                }
                            }
                        }
                        else if (setting.StartsWith("Mappings="))
                        {
                            //Load mappings string
                            mappingString = setting.Substring(setting.IndexOf(RANDO_OPTION_SETTING_VALUE_DELIM) + 1);
                            
                        }
                        else //Other settings
                        {

                            string[] splitSeedSetting = setting.Split(RANDO_OPTION_SETTING_VALUE_DELIM);
                            if (splitSeedSetting.Length == 2)
                            {
                                string splitSeedSettingType = splitSeedSetting[0];
                                string splitSeedSettingValue = splitSeedSetting[1];

                                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Split setting '{splitSeedSettingType}' with value '{splitSeedSettingValue}' added to seed split {i}");

                                if ((splitSeedSettingType != null && Enum.IsDefined(typeof(SettingType), splitSeedSettingType)) && (splitSeedSettingValue != null && Enum.IsDefined(typeof(SettingValue), splitSeedSettingValue)))
                                {
                                    seedSettings.Add((SettingType)Enum.Parse(typeof(SettingType), splitSeedSettingType), (SettingValue)Enum.Parse(typeof(SettingValue), splitSeedSettingValue));
                                }
                            }
                            else
                            {
                                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"ERROR WHILE LOADING FROM MOD SAVE FILE: Setting not properly formatted in file. Setting in question '{setting}'. Throwing it away and moving on.");
                            }
                        }
                    }
                }

                stateManager.AddSeed(i, seedType, seed, seedSettings, collectedRandoItemList, mappingString);
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"'{seeds[i]}' added to state manager successfully.");
            }

            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "Loading into state manager complete.");
        }
    }
    
}
