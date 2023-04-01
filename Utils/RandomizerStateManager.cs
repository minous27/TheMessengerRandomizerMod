using System.Collections.Generic;
using MessengerRando.Utils;
using MessengerRando.RO;
using Mod.Courier;


namespace MessengerRando
{
    class RandomizerStateManager
    {
        public static RandomizerStateManager Instance { private set; get; }
        public Dictionary<LocationRO, RandoItemRO> CurrentLocationToItemMapping { set; get; }
        public Dictionary<int, List<string>> DefeatedBosses;

        public bool IsRandomizedFile { set; get; }
        public int CurrentFileSlot { set; get; }

        private Dictionary<int, SeedRO> seeds;

        private Dictionary<EItems, bool> noteCutsceneTriggerStates;
        public Dictionary<string, string> CurrentLocationDialogtoRandomDialogMapping { set; get; }
        //This overrides list will be used to track items that, during the giving of items in any particular moment, need to ignore rando logic and just hand the item over.
        private List<EItems> temporaryRandoOverrides;

        public static void Initialize()
        {
            if(Instance == null)
            {
                Instance = new RandomizerStateManager();
            }
        }

        private RandomizerStateManager()
        {
            //Create initial values for the state machine
            this.seeds = new Dictionary<int, SeedRO>();

            

            this.ResetRandomizerState();
            this.initializeCutsceneTriggerStates();
            this.temporaryRandoOverrides = new List<EItems>();
        }

        private void initializeCutsceneTriggerStates()
        {
            noteCutsceneTriggerStates = new Dictionary<EItems, bool>();
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_CHAOS, false);
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_COURAGE, false);
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_HOPE, false);
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_LOVE, false);
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_STRENGTH, false);
            noteCutsceneTriggerStates.Add(EItems.KEY_OF_SYMBIOSIS, false);
        }

        /// <summary>
        /// Add seed to state's collection of seeds, providing all the necessary info to create the SeedRO object.
        /// </summary>
        public void AddSeed(int fileSlot, SeedType seedType, int seed, Dictionary<SettingType, SettingValue> settings, List<RandoItemRO> collectedItems, string mappingJson)
        {
            AddSeed(new SeedRO(fileSlot, seedType, seed, settings, collectedItems, mappingJson));
        }

        /// <summary>
        /// Add seed to state's collection of seeds.
        /// </summary>
        public void AddSeed(SeedRO seed)
        {
            seeds[seed.FileSlot] = seed;
        }

        public SeedRO GetSeedForFileSlot(int fileSlot)
        {
            if (!seeds.ContainsKey(fileSlot))
            {
                seeds[fileSlot] = new SeedRO(fileSlot, SeedType.None, 0, null, null, null);
            }
            return seeds[fileSlot];
        }

        /// <summary>
        /// Reset's the state's seed for the provided file slot. This will replace the seed with an empty seed, telling the mod this fileslot is not randomized.
        /// </summary>
        public void ResetSeedForFileSlot(int fileSlot)
        {
            //Simply keeping resetting logic here in case I want to change it i'll only do so here
            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Resetting file slot '{fileSlot}'");
            if (seeds.ContainsKey(fileSlot))
            {
                seeds[fileSlot] = new SeedRO(fileSlot, SeedType.None, 0, null, null, null);
                if (DefeatedBosses == null) DefeatedBosses = new Dictionary<int, List<string>>();
                DefeatedBosses[fileSlot] = new List<string>();
            }
            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "File slot reset complete.");
        }

        /// <summary>
        /// Checks to see if a seed exists for the given file slot.
        /// </summary>
        /// <returns>true if a seed was found and that the seed has a non-zero seed number and that seed does not have a NONE seed type. False otherwise.</returns>
        public bool HasSeedForFileSlot(int fileSlot)
        {
            bool seedFound = false;

            if(this.seeds.ContainsKey(fileSlot) && this.seeds[fileSlot].Seed != 0 && this.seeds[fileSlot].SeedType != SeedType.None)
            {
                seedFound = true;
            }

            return seedFound;
        }

        public void ResetRandomizerState()
        {
            CurrentLocationToItemMapping = new Dictionary<LocationRO, RandoItemRO>();
            this.IsRandomizedFile = false;
        }

        public bool IsNoteCutsceneTriggered(EItems note)
        {
            return this.noteCutsceneTriggerStates[note];
        }

        public void SetNoteCutsceneTriggered(EItems note)
        {
            this.noteCutsceneTriggerStates[note] = true;
        }

        public void AddTempRandoItemOverride(EItems randoItem)
        {
            temporaryRandoOverrides.Add(randoItem);
        }

        public void RemoveTempRandoItemOverride(EItems randoItem)
        {
            temporaryRandoOverrides.Remove(randoItem);
        }

        public bool HasTempOverrideOnRandoItem(EItems randoItem)
        {
            return temporaryRandoOverrides.Contains(randoItem);
        }

        public bool IsSafeTeleportState()
        {
            //Unsafe teleport states are shops/hq/boss fights
            bool isTeleportSafe = true;

            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"In ToT HQ: {Manager<TotHQ>.Instance.root.gameObject.activeInHierarchy}");
            CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"In Shop: {Manager<Shop>.Instance.gameObject.activeInHierarchy}");

            //ToT HQ or Shop
            if (Manager<TotHQ>.Instance.root.gameObject.activeInHierarchy || Manager<Shop>.Instance.gameObject.activeInHierarchy)
            {
                isTeleportSafe = false;
            }

            return isTeleportSafe;
        }

        /// <summary>
        /// Check through the mappings for any location that is represented by vanilla location item(since that is the key used to uniquely identify locations).
        /// </summary>
        /// <param name="vanillaLocationItem">EItem being used to look up location.</param>
        /// <param name="locationFromItem">Out parameter used to return the location found.</param>
        /// <returns>true if location was found, otherwise false(location item will be null in this case)</returns>
        public bool IsLocationRandomized(EItems vanillaLocationItem, out LocationRO locationFromItem)
        {
            bool isLocationRandomized = false;
            locationFromItem = null;

            //We'll check through notes first
            foreach (RandoItemRO note in RandomizerConstants.GetNotesList())
            {
                if (note.Item.Equals(vanillaLocationItem))
                {
                    
                    locationFromItem = new LocationRO(note.Name);

                    if (CurrentLocationToItemMapping.ContainsKey(locationFromItem))
                    {
                        isLocationRandomized = true;
                    }
                    else
                    {
                        //Then we know for certain it was not randomized. No reason to continue.
                        locationFromItem = null;
                        return false;
                    }
                }
            }

            //If it wasn't a note we'll look through the rest of the items
            if (!isLocationRandomized){
                
                //Real quick, check Climbing Claws because it is special
                if(EItems.CLIMBING_CLAWS.Equals(vanillaLocationItem))
                {
                    locationFromItem = new LocationRO("Climbing_Claws");
                    return true;
                }


                foreach (RandoItemRO item in RandomizerConstants.GetRandoItemList())
                {
                    if (item.Item.Equals(vanillaLocationItem))
                    { 

                        locationFromItem = new LocationRO(item.Name);

                        if (CurrentLocationToItemMapping.ContainsKey(locationFromItem))
                        {
                            isLocationRandomized = true;
                        }
                        else
                        {
                            //Then we know for certain it was not randomized.
                            locationFromItem = null;
                            return false;
                        }

                    }
                }
            }

            //Return whether we found it or not.
            return isLocationRandomized;
        }

        /// <summary>
        /// Helper method to log out the current mappings all nicely for review
        /// </summary>
        public void LogCurrentMappings()
        {
            if(this.CurrentLocationToItemMapping != null)
            {
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "----------------BEGIN Current Mappings----------------");
                foreach (LocationRO check in this.CurrentLocationToItemMapping.Keys)
                {
                    CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Check '{check.PrettyLocationName}'({check.LocationName}) contains Item '{this.CurrentLocationToItemMapping[check]}'");
                }
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "----------------END Current Mappings----------------");
            }
            else
            {
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "Location mappings were not set for this seed.");
            }

            if (CurrentLocationDialogtoRandomDialogMapping != null)
            {
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "----------------BEGIN Current Dialog Mappings----------------");
                foreach (KeyValuePair<string, string> KVP in CurrentLocationDialogtoRandomDialogMapping)
                {
                    CourierLogger.Log(RandomizerConstants.LOGGER_TAG, $"Dialog '{KVP.Value}' is located at Check '{KVP.Key}'");
                }
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "----------------END Current Dialog Mappings----------------");
            }
            else
            {
                CourierLogger.Log(RandomizerConstants.LOGGER_TAG, "Dialog mappings were not set for this seed.");
            }
        }

    }
}
