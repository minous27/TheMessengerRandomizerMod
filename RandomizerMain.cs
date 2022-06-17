﻿using System;
using System.Collections.Generic;
using MessengerRando.Overrides;
using MessengerRando.Utils;
using MessengerRando.RO;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.UI;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Mod.Courier.UI.TextEntryButtonInfo;
using MessengerRando.Exceptions;

namespace MessengerRando 
{
    /// <summary>
    /// Where it all begins! This class defines and injects all the necessary for the mod.
    /// </summary>
    public class RandomizerMain : CourierModule
    {
        private const string RANDO_OPTION_KEY = "minous27RandoSeeds";
        private const int MAX_BEATABLE_SEED_ATTEMPTS = 1;

        private RandomizerStateManager randoStateManager;
        private RandomizerSaveMethod randomizerSaveMethod;

        TextEntryButtonInfo loadRandomizerFileForFileSlotButton;
  
        SubMenuButtonInfo versionButton;

        SubMenuButtonInfo windmillShurikenToggleButton;
        SubMenuButtonInfo teleportToHqButton;
        SubMenuButtonInfo teleportToNinjaVillage;

        //Set up save data
        public override Type ModuleSaveType => typeof(RandoSave);
        public RandoSave Save => (RandoSave)ModuleSave;

        public override void Load()
        {
            Console.WriteLine("Randomizer loading and ready to try things!");
          
            //Initialize the randomizer state manager
            RandomizerStateManager.Initialize();
            randoStateManager = RandomizerStateManager.Instance;

            //Set up save data utility
            randomizerSaveMethod = new RandomizerSaveMethod();

            //Add Randomizer Version button
            versionButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Messenger Randomizer: v" + ItemRandomizerUtil.GetModVersion(), null);

            //Add generate random seed mod option button
            loadRandomizerFileForFileSlotButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Load Randomizer File For File Slot", (entry) => OnEnterFileSlot(entry, Int32.MinValue, SeedType.No_Logic), 1, () => "Which save slot would you like to start a rando seed?", () => "1", CharsetFlags.Number);

            //Add windmill shuriken toggle button
            windmillShurikenToggleButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<ProgressionManager>.Instance.useWindmillShuriken ? "Active Regular Shurikens" : "Active Windmill Shurikens", OnToggleWindmillShuriken);

            //Add teleport to HQ button
            teleportToHqButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Teleport to HQ", OnSelectTeleportToHq);

            //Add teleport to Ninja Village button
            teleportToNinjaVillage = Courier.UI.RegisterSubMenuModOptionButton(() => "Teleport to Ninja Village", OnSelectTeleportToNinjaVillage);


            //Plug in my code :3
            On.InventoryManager.AddItem += InventoryManager_AddItem;
            On.ProgressionManager.SetChallengeRoomAsCompleted += ProgressionManager_SetChallengeRoomAsCompleted;
            On.HasItem.IsTrue += HasItem_IsTrue;
            On.AwardNoteCutscene.ShouldPlay += AwardNoteCutscene_ShouldPlay;
            On.CutsceneHasPlayed.IsTrue += CutsceneHasPlayed_IsTrue;
            On.SaveGameSelectionScreen.OnLoadGame += SaveGameSelectionScreen_OnLoadGame;
            On.SaveGameSelectionScreen.OnNewGame += SaveGameSelectionScreen_OnNewGame;
            On.NecrophobicWorkerCutscene.Play += NecrophobicWorkerCutscene_Play;
            IL.RuxxtinNoteAndAwardAmuletCutscene.Play += RuxxtinNoteAndAwardAmuletCutscene_Play;
            On.CatacombLevelInitializer.OnBeforeInitDone += CatacombLevelInitializer_OnBeforeInitDone;
            On.DialogManager.LoadDialogs_ELanguage += DialogChanger.LoadDialogs_Elanguage;
            //temp add
            On.PowerSeal.OnEnterRoom += PowerSeal_OnEnterRoom;
            On.DialogSequence.GetDialogList += DialogSequence_GetDialogList;

            Console.WriteLine("Randomizer finished loading!");
        }

        public override void Initialize()
        {
            //I only want the generate seed/enter seed mod options available when not in the game.
            loadRandomizerFileForFileSlotButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;

            //Options I only want working while actually in the game
            windmillShurikenToggleButton.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.WINDMILL_SHURIKEN) > 0);
            teleportToHqButton.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && randoStateManager.IsSafeTeleportState());
            teleportToNinjaVillage.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && Manager<ProgressionManager>.Instance.HasCutscenePlayed("ElderAwardSeedCutscene") && randoStateManager.IsSafeTeleportState()); 

            //Options always available
            versionButton.IsEnabled = () => true;
            
            //Save loading
            Debug.Log("Start loading seeds from save");
            randomizerSaveMethod.Load(Save.seedData);
            Debug.Log($"Save data after change: '{Save.seedData}'");
            Debug.Log("Finished loading seeds from save");
        }

        //temp function for seal research
        void PowerSeal_OnEnterRoom(On.PowerSeal.orig_OnEnterRoom orig, PowerSeal self, bool teleportedInRoom)
        {
            //just print out some info for me
            Console.WriteLine($"Entered power seal room: {Manager<Level>.Instance.GetRoomAtPosition(self.transform.position).roomKey}");
            orig(self, teleportedInRoom);
        }

        List<DialogInfo> DialogSequence_GetDialogList(On.DialogSequence.orig_GetDialogList orig, DialogSequence self)
        {
            if(randoStateManager.IsRandomizedFile && (self.dialogID == "RANDO_ITEM" || self.dialogID == "SEED_NOT_BEATABLE"))
            {
                Console.WriteLine("Trying some rando dialog stuff.");
                List<DialogInfo> dialogInfoList = new List<DialogInfo>();
                DialogInfo dialog = new DialogInfo();
                switch (self.dialogID)
                {
                    case "RANDO_ITEM":
                        dialog.text = $"You have received item: '{self.name}'";
                        break;
                    case "SEED_NOT_BEATABLE":
                        //TODO remove
                        dialog.text = $"Seed '{self.name}' was not beatable. Submit again to retry.";
                        break;
                    default:
                        dialog.text = "???";
                        break;
                }
                    
                
                dialogInfoList.Add(dialog);

                return dialogInfoList;
            }

            return orig(self);
        }

        void InventoryManager_AddItem(On.InventoryManager.orig_AddItem orig, InventoryManager self, EItems itemId, int quantity)
        {

            LocationRO randoItemCheck = new LocationRO(itemId.ToString());

            if(itemId != EItems.TIME_SHARD) //killing the timeshard noise in the logs
            {
                Console.WriteLine($"Called InventoryManager_AddItem method. Looking to give x{quantity} amount of item '{itemId}'.");
            }

            //Lets make sure that the item they are collecting is supposed to be randomized
            if (randoStateManager.IsRandomizedFile && !RandomizerStateManager.Instance.HasTempOverrideOnRandoItem(itemId) && (randoStateManager.CurrentLocationToItemMapping.ContainsKey(randoItemCheck)))
            {
                //Based on the item that is attempting to be added, determine what SHOULD be added instead
                RandoItemRO randoItemId = randoStateManager.CurrentLocationToItemMapping[randoItemCheck];
                Console.WriteLine($"Randomizer magic engage! Game wants item '{itemId}', giving it rando item '{randoItemId}' with a quantity of '{quantity}'");
                
                //If that item is the windmill shuriken, immediately activate it and the mod option
                if(EItems.WINDMILL_SHURIKEN.Equals(randoItemId.Item))
                {
                    OnToggleWindmillShuriken();
                }
                else if (EItems.TIME_SHARD.Equals(randoItemId.Item)) //Handle timeshards
                {
                    Manager<InventoryManager>.Instance.CollectTimeShard(quantity);
                    randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItemId);
                    return; //Collecting timeshards internally call add item so I dont need to do it again.
                }

                //Set the itemId to the new item
                itemId = randoItemId.Item;
                //Set this item to have been collected in the state manager
                randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(randoItemId);

                //Save
                Save.seedData = randomizerSaveMethod.GenerateSaveData();
            }
            
            //Call original add with items
            orig(self, itemId, quantity);
            
        }

        void ProgressionManager_SetChallengeRoomAsCompleted(On.ProgressionManager.orig_SetChallengeRoomAsCompleted orig, ProgressionManager self, string roomKey)
        {
            //if this is a rando file, go ahead and give the item we expect to get
            if (randoStateManager.IsRandomizedFile)
            {
                LocationRO powerSealLocation = null;
                foreach(LocationRO location in RandomizerConstants.GetAdvancedRandoLocationList())
                {
                    if(location.LocationName.Equals(roomKey))
                    {
                        powerSealLocation = location;
                    }
                }

                if(powerSealLocation == null)
                {
                    throw new RandomizerException($"Challenge room with room key '{roomKey}' was not found in the list of locations. This will need to be corrected for this challenge room to work.");
                }

                RandoItemRO challengeRoomRandoItem = RandomizerStateManager.Instance.CurrentLocationToItemMapping[powerSealLocation];

                Console.WriteLine($"Challenge room '{powerSealLocation.PrettyLocationName}' completed. Providing rando item '{challengeRoomRandoItem}'.");
                //Handle timeshards
                if (EItems.TIME_SHARD.Equals(challengeRoomRandoItem.Item))
                {
                    Manager<InventoryManager>.Instance.CollectTimeShard(1);
                    //Set this item to have been collected in the state manager
                    randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Add(challengeRoomRandoItem);
                }
                else
                {
                    //Before adding the item to the inventory, add this item to the override
                    RandomizerStateManager.Instance.AddTempRandoItemOverride(challengeRoomRandoItem.Item);
                    Manager<InventoryManager>.Instance.AddItem(challengeRoomRandoItem.Item, 1);
                    //Now remove the override
                    RandomizerStateManager.Instance.RemoveTempRandoItemOverride(challengeRoomRandoItem.Item);
                }

                //I want to try to have a dialog popup say what the player got.
                DialogSequence challengeSequence = ScriptableObject.CreateInstance<DialogSequence>();
                challengeSequence.dialogID = "RANDO_ITEM";
                challengeSequence.name = challengeRoomRandoItem.Item.ToString();
                challengeSequence.choices = new List<DialogSequenceChoice>();
                AwardItemPopupParams challengeAwardItemParams = new AwardItemPopupParams(challengeSequence, true);
                Manager<UIManager>.Instance.ShowView<AwardItemPopup>(EScreenLayers.PROMPT, challengeAwardItemParams, true);


            }


            //For now calling the orig method once we are done so the game still things we are collecting seals. We can change this later.
            orig(self, roomKey);
        }

        bool HasItem_IsTrue(On.HasItem.orig_IsTrue orig, HasItem self)
        {
            bool hasItem = false;
            LocationRO check = new LocationRO(self.item.ToString());
            //Check to make sure this is an item that was randomized and make sure we are not ignoring this specific trigger check
            if (randoStateManager.IsRandomizedFile && RandomizerConstants.GetRandoLocationList().Contains(check) && !RandomizerConstants.GetSpecialTriggerNames().Contains(self.Owner.name))
            {
                if (self.transform.parent != null && "InteractionZone".Equals(self.Owner.name) && RandomizerConstants.GetSpecialTriggerNames().Contains(self.transform.parent.name) && EItems.KEY_OF_LOVE != self.item)
                {
                    //Special triggers that need to use normal logic, call orig method. This also includes the trigger check for the key of love on the sunken door because yeah.
                    Console.WriteLine($"While checking if player HasItem in an interaction zone, found parent object '{self.transform.parent.name}' in ignore logic. Calling orig HasItem logic.");
                    return orig(self);
                }

                //OLD WAY
                //Don't actually check for the item i have, check to see if I have the item that was at it's location.
                //int itemQuantity = Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[check].Item);

                //NEW WAY
                //Don't actually check for the item I have, check to see if I have done this check before. We'll do this by seeing if the item at its location has been collected yet or not
                int itemQuantity = randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[check]) ? randoStateManager.CurrentLocationToItemMapping[check].Quantity : 0;
                
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

                Console.WriteLine($"Rando inventory check complete for check '{self.Owner.name}'. Item '{self.item}' || Actual Item Check '{randoStateManager.CurrentLocationToItemMapping[check]}' || Current Check '{self.conditionOperator}' || Expected Quantity '{self.quantityToHave}' || Actual Quantity '{itemQuantity}' || Condition Result '{hasItem}'.");
                
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
            LocationRO noteCheck = new LocationRO(self.noteToAward.ToString());
            if (randoStateManager.IsRandomizedFile && randoStateManager.CurrentLocationToItemMapping.ContainsKey(noteCheck)) //Double checking to prevent errors
            {
                Console.WriteLine($"Note cutscene check! Handling note '{self.noteToAward}' | Linked item: '{randoStateManager.CurrentLocationToItemMapping[noteCheck]}'");
                //bool shouldPlay = Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[noteCheck].Item) <= 0 && !randoStateManager.IsNoteCutsceneTriggered(self.noteToAward);
                bool shouldPlay = !randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[noteCheck]) && !randoStateManager.IsNoteCutsceneTriggered(self.noteToAward);

                Console.WriteLine($"Should '{self.noteToAward}' cutscene play? '{shouldPlay}'");
                
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
            
            if(randoStateManager.IsRandomizedFile && RandomizerConstants.GetCutsceneMappings().ContainsKey(self.cutsceneId))
            {
                LocationRO cutsceneCheck = new LocationRO(RandomizerConstants.GetCutsceneMappings()[self.cutsceneId].ToString());

                //Check to make sure this is a cutscene i am configured to check, then check to make sure I actually have the item that is mapped to it
                Console.WriteLine($"Rando cutscene magic ahoy! Handling rando cutscene '{self.cutsceneId}' | Linked Item: {RandomizerConstants.GetCutsceneMappings()[self.cutsceneId]} | Rando Item: {randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}");

                

                //Check to see if I have the item that is at this check.
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[cutsceneCheck].Item) >= 1)
                if(randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]))
                {
                    //Return true, this cutscene has "been played"
                    Console.WriteLine($"Have rando item '{randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}' for cutscene '{self.cutsceneId}'. Returning that we have already seen cutscene.");
                    return self.mustHavePlayed == true;
                }
                else
                {
                    //Havent seen the cutscene yet. Play it so i can get the item!
                    Console.WriteLine($"Do not have rando item '{randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}' for cutscene '{self.cutsceneId}'. Returning that we have not seen cutscene yet.");
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
               
                //Load mappings
                randoStateManager.CurrentLocationToItemMapping = ItemRandomizerUtil.LoadMappings(randoStateManager.GetSeedForFileSlot(fileSlot));

                //for now, only turn on dialog mappings for basic seeds
                SettingValue currentDifficultySetting = SettingValue.Advanced;
                randoStateManager.GetSeedForFileSlot(fileSlot).Settings.TryGetValue(SettingType.Difficulty, out currentDifficultySetting);
                if (currentDifficultySetting.Equals(SettingValue.Basic))
                {
                    randoStateManager.CurrentLocationDialogtoRandomDialogMapping = DialogChanger.GenerateDialogMappingforItems();
                }
 
                randoStateManager.IsRandomizedFile = true;
                randoStateManager.CurrentFileSlot = fileSlot;
                //Log spoiler log
                randoStateManager.LogCurrentMappings();

                //We force a reload of all dialog when loading the game
                Manager<DialogManager>.Instance.LoadDialogs(Manager<LocalizationManager>.Instance.CurrentLanguage);
            }
            else
            {
                //This save file does not have a seed associated with it or is not a randomized file. Reset the mappings so everything is back to normal.
                Console.WriteLine($"This file slot ({fileSlot}) has no seed generated or is not a randomized file. Resetting the mappings and putting game items back to normal.");
                randoStateManager.ResetRandomizerState();
            }
            orig(self, slotIndex);
        }

        void SaveGameSelectionScreen_OnNewGame(On.SaveGameSelectionScreen.orig_OnNewGame orig, SaveGameSelectionScreen self, SaveSlotUI slot)
        {
            //Right now I am not randomizing game slots that are brand new.
            Console.WriteLine($"This file slot is brand new. Resetting the mappings and putting game items back to normal.");
            randoStateManager.ResetRandomizerState();
            randoStateManager.ResetSeedForFileSlot(slot.slotIndex + 1);

            orig(self, slot);
        }

        //Fixing necro cutscene check
        void CatacombLevelInitializer_OnBeforeInitDone(On.CatacombLevelInitializer.orig_OnBeforeInitDone orig, CatacombLevelInitializer self)
        {
            
            if(randoStateManager.IsRandomizedFile)
            {
                //check to see if we already have the item at Necro check
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())].Item) <= 0 && !Manager<DemoManager>.Instance.demoMode)
                if (!randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())]) && !Manager<DemoManager>.Instance.demoMode)
                {
                    //Run the cutscene if we dont
                    Console.WriteLine($"Have not received item '{randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())]}' from Necro check. Playing cutscene.");
                    self.necrophobicWorkerCutscene.Play();
                }
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())].Item) >= 1 || Manager<DemoManager>.Instance.demoMode)
                if (randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())]) || Manager<DemoManager>.Instance.demoMode)
                {
                    //set necro inactive if we do
                    Console.WriteLine($"Already have item '{randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())]}' from Necro check. Will not play cutscene.");
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

        ///On submit of rando file location
        bool OnEnterFileSlot(string fileSlot, int seed, SeedType seedType)
        {
            Console.WriteLine($"In Method: OnEnterFileSlot. Provided value: '{fileSlot}'");
            Console.WriteLine($"Received file slot number: {fileSlot}");
            int slot = Convert.ToInt32(fileSlot);
            if (slot < 1 || slot > 3)
            {
                Console.WriteLine($"Invalid slot number provided: {slot}");
                return false;
            }

            //Load in mappings and save them to the state
            string mappingString = ItemRandomizerUtil.LoadMappingsFromFile(slot);
            Console.WriteLine($"File reading complete. Received the following mappings: '{mappingString}'");
            SeedRO seedRO = randoStateManager.GetSeedForFileSlot(slot);
            seedRO.MappingB64 = mappingString;
            randoStateManager.AddSeed(seedRO);

            //********TODO REMOVE*********
            ItemRandomizerUtil.LoadMappings(seedRO);

            //Save
            Save.seedData = randomizerSaveMethod.GenerateSaveData();

            return true;
        }

        void OnToggleWindmillShuriken()
        {
            //Toggle Shuriken
            Manager<ProgressionManager>.Instance.useWindmillShuriken = !Manager<ProgressionManager>.Instance.useWindmillShuriken;
            //Update UI
            InGameHud view = Manager<UIManager>.Instance.GetView<InGameHud>();
            if (view != null)
            {
                view.UpdateShurikenVisibility();
            }
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

        void OnSelectTeleportToNinjaVillage()
        {
            Console.WriteLine("Attempting to teleport to Ninja Village.");
            
            // Properly close out of the mod options and get the game state back together
            Manager<PauseManager>.Instance.Resume();
            Manager<UIManager>.Instance.GetView<OptionScreen>().Close(false);
            Courier.UI.ModOptionScreen.Close(false);
            EBits dimension = Manager<DimensionManager>.Instance.currentDimension;

            //Fade the music out because musiception is annoying
            Manager<AudioManager>.Instance.FadeMusicVolume(1f, 0f, true);

            //Load to Ninja Village
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = new Vector2(-153.3f, -56.5f);
            LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo("Level_01_NinjaVillage_Build", false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, dimension);
            Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
            
            Console.WriteLine("Teleport to Ninja Village complete.");
        }

        /// <summary>
        /// Delegate function for getting rando item. This can be used by IL hooks that need to make this call later.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private EItems GetRandoItemByItem(EItems item)
        {
            Console.WriteLine($"IL Wackiness -- Checking for Item '{item}' | Rando item to return '{randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.RUXXTIN_AMULET.ToString())]}'");
            return randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.RUXXTIN_AMULET.ToString())].Item;
        }
    }
}
