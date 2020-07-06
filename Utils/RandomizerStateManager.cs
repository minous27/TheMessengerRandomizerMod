using System;
using System.Collections.Generic;


namespace MessengerRando
{
    class RandomizerStateManager
    {
        public static RandomizerStateManager Instance { private set; get; }
        public Dictionary<EItems, EItems> CurrentLocationToItemMapping { set; get; }
        public bool IsRandomizedFile { set; get; }
        
        private Dictionary<int, int> seeds;

        private Dictionary<EItems, bool> noteCutsceneTriggerStates;


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
            this.seeds = new Dictionary<int, int>();
            this.ResetCurrentLocationToItemMappings();
            this.initializeCutsceneTriggerStates();
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

        public void AddSeed(int fileSlot, int seed)
        {
            seeds[fileSlot] = seed;
        }

        public int GetSeedForFileSlot(int fileSlot)
        {
            int seed = 0;

            if (seeds.ContainsKey(fileSlot))
            {
                seed = seeds[fileSlot];
            }

            return seed;
        }

        public void ResetSeedForFileSlot(int fileSlot)
        {
            //Simply keeping resetting logic here in case I want to change it i'll only do so here
            Console.WriteLine($"Resetting file slot '{fileSlot}'");
            if (seeds.ContainsKey(fileSlot))
            {
                seeds[fileSlot] = 0;
            }
            Console.WriteLine("File slot reset complete.");
        }

        public bool HasSeedForFileSlot(int fileSlot)
        {
            bool seedFound = false;

            if(this.seeds.ContainsKey(fileSlot) && seeds[fileSlot] != 0)
            {
                seedFound = true;
            }

            return seedFound;
        }

        public void ResetCurrentLocationToItemMappings()
        {
            CurrentLocationToItemMapping = new Dictionary<EItems, EItems>();
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

        public bool IsSafeTeleportState()
        {
            //Unsafe teleport states are shops/hq/boss fights

            //Manager<LevelManager>.Instance.CurrentSceneName
            
            return true;
        }

    }
}
