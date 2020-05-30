using System;
using MessengerRando.Overrides;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.UI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.EventSystems;
using static Mod.Courier.UI.TextEntryButtonInfo;


namespace MessengerRando 
{
    public class RandomizedItemInserter : CourierModule
    {
        private const string RANDO_OPTION_KEY = "minous27RandoSeeds";

        private RandomizerStateManager randoStateManager;       

        TextEntryButtonInfo generateSeedButton;
        TextEntryButtonInfo enterSeedButton;
        SubMenuButtonInfo teleportToHqButton;

        public override void Load()
        {
            Console.WriteLine("Randomizer loaded and ready to try things!");
           
            //Start the randomizer util initializations
            ItemRandomizerUtil.Load();
            //Initialize the randomizer state manager
            RandomizerStateManager.Initialize();
            randoStateManager = RandomizerStateManager.Instance;
            RandomizerSaveMethod randomizerSaveMethod = new RandomizerSaveMethod(RANDO_OPTION_KEY);


            //Add generate mod option button
            generateSeedButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Generate Random Seed", OnEnterRandoFileSlot, 1, () => "Which save slot would you like to start a rando seed?", () => "1", CharsetFlags.Number);
            generateSeedButton.SaveMethod = randomizerSaveMethod;

            //Add Set seed mod option button
            enterSeedButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Set Randomizer Seed", OnEnterSeedNumber, 15, () => "What is the seed you would like to play?", null,  CharsetFlags.Number);
            generateSeedButton.SaveMethod = randomizerSaveMethod;

            //Add teleport to HQ button\
            teleportToHqButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Teleport to HQ", OnSelectTeleportToHq);

            //Plug in my code :3
            On.InventoryManager.AddItem += InventoryManager_AddItem;
            On.HasItem.IsTrue += HasItem_IsTrue;
            On.AwardNoteCutscene.ShouldPlay += AwardNoteCutscene_ShouldPlay;
            On.CutsceneHasPlayed.IsTrue += CutsceneHasPlayed_IsTrue;
            On.SaveGameSelectionScreen.OnLoadGame += SaveGameSelectionScreen_OnLoadGame;
            On.SaveGameSelectionScreen.OnNewGame += SaveGameSelectionScreen_OnNewGame;
            On.PhantomEnemy.ReceiveHit += PhantomEnemy_ReceiveHit;
            On.NecrophobicWorkerCutscene.Play += NecrophobicWorkerCutscene_Play;
            IL.RuxxtinNoteAndAwardAmuletCutscene.Play += RuxxtinNoteAndAwardAmuletCutscene_Play;
            On.CatacombLevelInitializer.OnBeforeInitDone += CatacombLevelInitializer_OnBeforeInitDone;

            Console.WriteLine("Randomizer finished loading!");
        }

        public override void Initialize()
        {
            //I only want the generate seed/enter seed mod options available when not in the game.
            generateSeedButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            enterSeedButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;

            teleportToHqButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE;
        }

        void InventoryManager_AddItem(On.InventoryManager.orig_AddItem orig, InventoryManager self, EItems itemId, int quantity)
        {
            //Currently defaulting rando values in case this is not a randomized item like pickups
            EItems randoItemId = itemId;

            Console.WriteLine($"Called InventoryManager_AddItem method. Looking to give x{quantity} amount of item '{itemId}'.");
            //Lets make sure that the item they are collecting is supposed to be randomized
            if (randoStateManager.IsRandomizedFile && randoStateManager.CurrentLocationToItemMapping.ContainsKey(randoItemId))
            {
                //Based on the item that is attempting to be added, determine what SHOULD be added instead
                randoItemId = randoStateManager.CurrentLocationToItemMapping[itemId];
                Console.WriteLine($"Randomizer magic engage! Game wants item '{itemId}', giving it rando item '{randoItemId}' with a quantity of '{quantity}'");
            }
            
            //Call original add with items
            orig(self, randoItemId, quantity);
        }

        bool HasItem_IsTrue(On.HasItem.orig_IsTrue orig, HasItem self)
        {
            bool hasItem = false;
            //Check to make sure this is an item that was randomized and make sure we are not ignoring this specific trigger check
            if (randoStateManager.IsRandomizedFile && ItemRandomizerUtil.RandomizableLocations.Contains(self.item) && !ItemRandomizerUtil.TriggersToIgnoreRandoItemLogic.Contains(self.Owner.name))
            {
                if (self.transform.parent != null && "InteractionZone".Equals(self.Owner.name) && ItemRandomizerUtil.TriggersToIgnoreRandoItemLogic.Contains(self.transform.parent.name) && EItems.KEY_OF_LOVE != self.item)
                {
                    //Special triggers that need to use normal logic, call orig method. This also includes the trigger check for the key of love on the sunken door because yeah.
                    Console.WriteLine($"While checking if player HasItem in an interaction zone, found parent object '{self.transform.parent.name}' in ignore logic. Calling orig HasItem logic.");
                    return orig(self);
                }

                //Don't actually check for the item i have, check to see if I have the item that was at it's location.
                int itemQuantity = Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[self.item]);
                switch (self.conditionOperator)
                {
                    case EConditionOperator.LESS_THAN:
                        hasItem = itemQuantity < self.quantityToHave;
                        break;
                    case EConditionOperator.LESS_OR_EQUAL:
                        hasItem = itemQuantity <= self.quantityToHave;
                        break;
                    case EConditionOperator.EQUAL:
                        hasItem = itemQuantity == self.quantityToHave;
                        break;
                    case EConditionOperator.GREATER_OR_EQUAL:
                        hasItem = itemQuantity >= self.quantityToHave;
                        break;
                    case EConditionOperator.GREATER_THAN:
                        hasItem = itemQuantity > self.quantityToHave;
                        break;
                }
                Console.WriteLine($"Rando inventory check complete for check '{self.Owner.name}'. Item '{self.item}' || Actual Item Check '{randoStateManager.CurrentLocationToItemMapping[self.item]}' || Current Check '{self.conditionOperator}' || Quantity '{itemQuantity}' || Condition Result '{hasItem}'.");
                return hasItem;
            }
            else //Call orig method
            {
                return orig(self);
            }
            
        }

        bool AwardNoteCutscene_ShouldPlay(On.AwardNoteCutscene.orig_ShouldPlay orig, AwardNoteCutscene self)
        {
            //Need to handle note cutscene triggers so they will play as long as I dont have the actual item it grants
            if (randoStateManager.IsRandomizedFile && randoStateManager.CurrentLocationToItemMapping.ContainsKey(self.noteToAward)) //Double checking to prevent errors
            {
                bool shouldPlay = Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[self.noteToAward]) <= 0 && !randoStateManager.IsNoteCutsceneTriggered(self.noteToAward);
                randoStateManager.SetNoteCutsceneTriggered(self.noteToAward);
                return shouldPlay;
            }
            else //Call orig method if for some reason the note I am checking for is not randomized
            {
                return orig(self);
            }
        }

        bool CutsceneHasPlayed_IsTrue(On.CutsceneHasPlayed.orig_IsTrue orig, CutsceneHasPlayed self)
        {
            
            if(randoStateManager.IsRandomizedFile && ItemRandomizerUtil.CutsceneMappings.ContainsKey(self.cutsceneId))
            {
                //Check to make sure this is a cutscene i am configured to check, then check to make sure I actually have the item that is mapped to it
                Console.WriteLine($"Rando cutscene magic ahoy! Handling rando cutscene '{self.cutsceneId}' | Linked Item: {ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]} | Rando Item: {randoStateManager.CurrentLocationToItemMapping[ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]]}");

                //Check to see if I have the item that is at this check.
                if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]]) >= 1)
                {
                    //Return true, this cutscene has "been played"
                    Console.WriteLine($"Have rando item '{randoStateManager.CurrentLocationToItemMapping[ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]]}' for cutscene '{self.cutsceneId}'. Returning that we have already seen cutscene.");
                    return self.mustHavePlayed == true;
                }
                else
                {
                    //Havent seen the cutscene yet. Play it so i can get the item!
                    Console.WriteLine($"Do not have rando item '{randoStateManager.CurrentLocationToItemMapping[ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]]}' for cutscene '{self.cutsceneId}'. Returning that we have not seen cutscene yet.");
                    return self.mustHavePlayed == false;
                }
            }
            else //call the orig method
            {
                return orig(self);
            }

            
        }

        void SaveGameSelectionScreen_OnLoadGame(On.SaveGameSelectionScreen.orig_OnLoadGame orig, SaveGameSelectionScreen self, int slotIndex)
        {

            //slotIndex is 0-based, going to increment it locally to keep things simple.
            int fileSlot = slotIndex + 1;
            //Generate the mappings based on the seed for the game if a seed was generated.
            if(randoStateManager.HasSeedForFileSlot(fileSlot))
            {
                Console.WriteLine($"Seed exists for file slot {fileSlot}. Generating mappings.");
                randoStateManager.CurrentLocationToItemMapping = ItemRandomizerUtil.GenerateRandomizedMappings(randoStateManager.GetSeedForFileSlot(fileSlot));
                randoStateManager.IsRandomizedFile = true;
            }
            else
            {
                //This save file does not have a seed associated with it. Reset the mappings so everything is back to normal.
                Console.WriteLine($"This file slot ({fileSlot}) has no seed generated. Resetting the mappings and putting game items back to normal.");
                randoStateManager.ResetCurrentLocationToItemMappings();
            }
            orig(self, slotIndex);
        }

        void SaveGameSelectionScreen_OnNewGame(On.SaveGameSelectionScreen.orig_OnNewGame orig, SaveGameSelectionScreen self, SaveSlotUI slot)
        {
            //Right now I am not randomizing game slots that are brand new.
            Console.WriteLine($"This file slot is brand new. Resetting the mappings and putting game items back to normal.");
            randoStateManager.ResetCurrentLocationToItemMappings();
            randoStateManager.ResetSeedForFileSlot(slot.slotIndex + 1);

            orig(self, slot);
        }

        //Phantom damage function. (TODO why bother doing this? xD)
        bool PhantomEnemy_ReceiveHit(On.PhantomEnemy.orig_ReceiveHit orig, PhantomEnemy self, HitData hitData)
        {
            //We want phantom to not take damage if all notes have not been collected yet.
            if((!randoStateManager.IsRandomizedFile) || ItemRandomizerUtil.HasAllNotes())
            {
                return orig(self, hitData);
            }
            //Didn't get all the notes...so nothing will happen!!!
            Console.WriteLine("I see you don't have all of the music notes...you thought you could damage phantom without them?!");
            return false;
        }

        //Fixing necro cutscene check
        void CatacombLevelInitializer_OnBeforeInitDone(On.CatacombLevelInitializer.orig_OnBeforeInitDone orig, CatacombLevelInitializer self)
        {
            
            if(randoStateManager.IsRandomizedFile)
            {
                //check to see if we already have the item at Necro check
                if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[EItems.NECROPHOBIC_WORKER]) <= 0 && !Manager<DemoManager>.Instance.demoMode)
                {
                    //Run the cutscene if we dont
                    Console.WriteLine($"Have not received item '{randoStateManager.CurrentLocationToItemMapping[EItems.NECROPHOBIC_WORKER]}' from Necro check. Playing cutscene.");
                    self.necrophobicWorkerCutscene.Play();
                }
                if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[EItems.NECROPHOBIC_WORKER]) >= 1 || Manager<DemoManager>.Instance.demoMode)
                {
                    //set necro inactive if we do
                    Console.WriteLine($"Already have item '{randoStateManager.CurrentLocationToItemMapping[EItems.NECROPHOBIC_WORKER]}' from Necro check. Will not play cutscene.");
                    self.necrophobicWorkerCutscene.phobekin.gameObject.SetActive(false);
                }
                //Call our overriden fixing function
                RandoCatacombLevelInitializer.FixPlayerStuckInChallengeRoom();
            }
            else
            {
                //we are not rando here, call orig method
                orig(self);
            }
            
        }

        // Breaking into Necro cutscene to fix things
        void NecrophobicWorkerCutscene_Play(On.NecrophobicWorkerCutscene.orig_Play orig, NecrophobicWorkerCutscene self)
        {
            //Cutscene moves Ninja around, lets see if i can stop it by making that "location" the current location the player is.
            self.playerStartPosition = UnityEngine.Object.FindObjectOfType<PlayerController>().transform;
            orig(self);
        }

        void RuxxtinNoteAndAwardAmuletCutscene_Play(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            while(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcI4(55)))
            {
                cursor.EmitDelegate<Func<EItems, EItems>>(GetRandoItemByItem);
            }
            
        }

        /*TODO Maybe use later, for now will not take a file name
        //On submit of rando file name
        bool OnEnterRandoFileName(string fileName)
        {
            Console.WriteLine($"File name received: {fileName}");
            randoFileName = fileName;

            //Pop up next input for which file slot to create this file in
            TextEntryPopup fileSlotPopup = InitTextEntryPopup(generateSeedButton.addedTo, "Which save slot would you like to start a rando seed?", (entry) => OnEnterRandoFileSlot(entry), 1, null, CharsetFlags.Number);
            fileSlotPopup.onBack += () =>
            {
                fileSlotPopup.gameObject.SetActive(false);
                generateSeedButton.textEntryPopup.gameObject.SetActive(true);
                generateSeedButton.textEntryPopup.StartCoroutine(generateSeedButton.textEntryPopup.BackWhenBackButtonReleased());
            };
            generateSeedButton.textEntryPopup.gameObject.SetActive(false);
            
            //Initialize the file slot popup
            fileSlotPopup.Init(string.Empty);
            fileSlotPopup.gameObject.SetActive(true);
            fileSlotPopup.transform.SetParent(generateSeedButton.addedTo.transform.parent);
            generateSeedButton.addedTo.gameObject.SetActive(false);
            Canvas.ForceUpdateCanvases();
            fileSlotPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = false;
            EventSystem.current.SetSelectedGameObject(fileSlotPopup.initialSelection);
            fileSlotPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = true;
            return false;
        }
        */

        bool OnEnterSeedNumber(string seed)
        {
            if(seed == null || seed.Length < 1)
            {
                Console.WriteLine($"Invalid seed number '{seed}' provided");
                return false;
            }

            TextEntryPopup fileSlotPopup = InitTextEntryPopup(enterSeedButton.addedTo, "Which save slot would you like to set this seed to?", (entry) => SetSeedForFileSlot(Convert.ToInt32(entry), Convert.ToInt32(seed)), 1, null, CharsetFlags.Number);
            fileSlotPopup.onBack += () => {
                fileSlotPopup.gameObject.SetActive(false);
                enterSeedButton.textEntryPopup.gameObject.SetActive(true);
                enterSeedButton.textEntryPopup.StartCoroutine(enterSeedButton.textEntryPopup.BackWhenBackButtonReleased());
            };
            enterSeedButton.textEntryPopup.gameObject.SetActive(false);
            fileSlotPopup.Init(string.Empty);
            fileSlotPopup.gameObject.SetActive(true);
            fileSlotPopup.transform.SetParent(enterSeedButton.addedTo.transform.parent);
            enterSeedButton.addedTo.gameObject.SetActive(false);
            Canvas.ForceUpdateCanvases();
            fileSlotPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = false;
            EventSystem.current.SetSelectedGameObject(fileSlotPopup.initialSelection);
            fileSlotPopup.initialSelection.GetComponent<UIObjectAudioHandler>().playAudio = true;
            return false;
        }

        //On submit of rando file location
        bool OnEnterRandoFileSlot(string fileSlot)
        {
            Console.WriteLine($"Received file slot number: {fileSlot}");

            return SetSeedForFileSlot(Convert.ToInt32(fileSlot));
        }

        //Moved the seed setting logic out so I could reuse it.
        bool SetSeedForFileSlot(int fileSlot, int seed = Int32.MinValue)
        {
            //Check to make sure an apporiate save slot was chosen.
            if (fileSlot < 1 || fileSlot > 3)
            {
                Console.WriteLine($"User provided an invalid save slot number {fileSlot}");
                return false;
            }

            if (seed == Int32.MinValue)
            {
                //Generate a seed
                Console.WriteLine("Generating seed...");
                seed = ItemRandomizerUtil.GenerateSeed();
                Console.WriteLine($"Seed generated: '{seed}'");
            }
            //Save this seed into the state
            randoStateManager.AddSeed(fileSlot, seed);

            Console.WriteLine($"Set seed '{seed}' to file slot '{fileSlot}'");
            return true;
        }

        void OnSelectTeleportToHq()
        {
            //Properly close out of the mod options and get the game state back together
            Manager<PauseManager>.Instance.Resume();
            Manager<UIManager>.Instance.GetView<OptionScreen>().Close(false);                
            Console.WriteLine("Teleporting to HQ!");
            Courier.UI.ModOptionScreen.Close(false);

            //Fade the music out because musiception is annoying
            Manager<AudioManager>.Instance.FadeMusicVolume(1f, 0f, true);

            //Load the HQ
            Manager<TowerOfTimeHQManager>.Instance.TeleportInToTHQ(true, ELevelEntranceID.ENTRANCE_A, null, null, true);
        }

        /// <summary>
        /// Delegate function for getting rando item. This can be used by IL hooks that need to make this call later.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private EItems GetRandoItemByItem(EItems item)
        {
            Console.WriteLine($"IL Wackiness -- Checking for Item '{item}' | Rando item to return '{randoStateManager.CurrentLocationToItemMapping[EItems.RUXXTIN_AMULET]}'");
            return randoStateManager.CurrentLocationToItemMapping[EItems.RUXXTIN_AMULET];
        }

    }
}
