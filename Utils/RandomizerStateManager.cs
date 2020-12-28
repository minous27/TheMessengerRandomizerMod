using System;
using System.Collections.Generic;
using MessengerRando.Utils;
using MessengerRando.RO;


namespace MessengerRando
{
    class RandomizerStateManager
    {
        public static RandomizerStateManager Instance { private set; get; }
        public Dictionary<LocationRO, RandoItemRO> CurrentLocationToItemMapping { set; get; }
        public bool IsRandomizedFile { set; get; }
        public int CurrentFileSlot { set; get; }

        private Dictionary<int, SeedRO> seeds;

        private Dictionary<EItems, bool> noteCutsceneTriggerStates;
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

        public void AddSeed(int fileSlot, SeedType seedType, int seed, Dictionary<SettingType, SettingValue> settings, List<RandoItemRO> collectedItems)
        {
            seeds[fileSlot] = new SeedRO(seedType, seed, settings, collectedItems);
        }

        public SeedRO GetSeedForFileSlot(int fileSlot)
        {
            if (!seeds.ContainsKey(fileSlot))
            {
                seeds[fileSlot] = new SeedRO(SeedType.None, 0, null, null);
            }
            return seeds[fileSlot];
        }

        public void ResetSeedForFileSlot(int fileSlot)
        {
            //Simply keeping resetting logic here in case I want to change it i'll only do so here
            Console.WriteLine($"Resetting file slot '{fileSlot}'");
            if (seeds.ContainsKey(fileSlot))
            {
                seeds[fileSlot] = new SeedRO(SeedType.None, 0, null, null);
            }
            Console.WriteLine("File slot reset complete.");
        }

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

            Console.WriteLine($"In ToT HQ: {Manager<TotHQ>.Instance.root.gameObject.activeInHierarchy}");
            Console.WriteLine($"In Shop: {Manager<Shop>.Instance.gameObject.activeInHierarchy}");

            //ToT HQ or Shop
            if (Manager<TotHQ>.Instance.root.gameObject.activeInHierarchy || Manager<Shop>.Instance.gameObject.activeInHierarchy)
            {
                isTeleportSafe = false;
            }

            return isTeleportSafe;
        }

        /// <summary>
        /// Helper method to log out the current mappings all nicely for review
        /// </summary>
        public void LogCurrentMappings()
        {
            Console.WriteLine("----------------BEGIN Current Mappings----------------");
            foreach(LocationRO check in this.CurrentLocationToItemMapping.Keys)
            {
                Console.WriteLine($"Check '{check.PrettyLocationName}' contains Item '{this.CurrentLocationToItemMapping[check]}'");
                //Console.WriteLine($"Item '{this.CurrentLocationToItemMapping[check]}' is located at Check '{check.PrettyLocationName}'");
            }
            Console.WriteLine("----------------END Current Mappings----------------");
        }

    }
}
