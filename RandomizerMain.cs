using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using MessengerRando.Archipelago;
using MessengerRando.Overrides;
using MessengerRando.Utils;
using MessengerRando.RO;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.UI;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Mod.Courier.UI.TextEntryButtonInfo;
using MessengerRando.Exceptions;
using TMPro;
using System.Linq;

namespace MessengerRando 
{
    /// <summary>
    /// Where it all begins! This class defines and injects all the necessary for the mod.
    /// </summary>
    public class RandomizerMain : CourierModule
    {
        private const string RANDO_OPTION_KEY = "minous27RandoSeeds";
        private const int MAX_BEATABLE_SEED_ATTEMPTS = 1;

        private float updateTimer;

        private RandomizerStateManager randoStateManager;
        private RandomizerSaveMethod randomizerSaveMethod;

        TextEntryButtonInfo loadRandomizerFileForFileSlotButton;
        TextEntryButtonInfo resetRandoSaveFileButton;
  
        SubMenuButtonInfo versionButton;
        SubMenuButtonInfo seedNumButton;

        SubMenuButtonInfo windmillShurikenToggleButton;
        SubMenuButtonInfo teleportToHqButton;
        SubMenuButtonInfo teleportToNinjaVillage;

        //Archipelago buttons
        SubMenuButtonInfo archipelagoHostButton;
        SubMenuButtonInfo archipelagoPortButton;
        SubMenuButtonInfo archipelagoNameButton;
        SubMenuButtonInfo archipelagoPassButton;
        SubMenuButtonInfo archipelagoConnectButton;

        public TextMeshProUGUI apMenu8;
        public TextMeshProUGUI apMenu16;

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

            //Add current seed number button
            seedNumButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Current seed number: " + GetCurrentSeedNum(), null);

            //Add load seed file button
            loadRandomizerFileForFileSlotButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Load Randomizer File For File Slot", (entry) => OnEnterFileSlot(entry), 1, () => "Which save slot would you like to start a rando seed?(1/2/3)", () => "1", CharsetFlags.Number);

            //Add Reset rando mod button
            resetRandoSaveFileButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Reset Randomizer File Slot", (entry) => OnRandoFileResetConfirmation(entry), 1, () => "Are you sure you wish to reset your save file for randomizer play?(y/n)", () => "n", CharsetFlags.Letter);

            //Add windmill shuriken toggle button
            windmillShurikenToggleButton = Courier.UI.RegisterSubMenuModOptionButton(() => Manager<ProgressionManager>.Instance.useWindmillShuriken ? "Active Regular Shurikens" : "Active Windmill Shurikens", OnToggleWindmillShuriken);

            //Add teleport to HQ button
            teleportToHqButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Teleport to HQ", OnSelectTeleportToHq);

            //Add teleport to Ninja Village button
            teleportToNinjaVillage = Courier.UI.RegisterSubMenuModOptionButton(() => "Teleport to Ninja Village", OnSelectTeleportToNinjaVillage);

            //Add Archipelago host button
            archipelagoHostButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Enter Archipelago Host Name", (entry) => OnSelectArchipelagoHost(entry), 30, () => "Enter the Archipelago host name. Use spaces for periods", () => "archipelago.gg");

            //Add Archipelago port button
            archipelagoPortButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Enter Archipelago Port", (entry) => OnSelectArchipelagoPort(entry), 5, () => "Enter the port for the Archipelago session", () => "38281", CharsetFlags.Number);

            //Add archipelago name button
            archipelagoNameButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Enter Archipelago Slot Name", (entry) => OnSelectArchipelagoName(entry), 16, () => "Enter player name:");

            //Add archipelago password button
            archipelagoPassButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Enter Archipelago Password", (entry) => OnSelectArchipelagoPass(entry), 30, () => "Enter session password:");

            //Add Archipelago connection button
            archipelagoConnectButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Connect to Archipelago", OnSelectArchipelagoConnect);


            //Plug in my code :3
            On.InventoryManager.AddItem += InventoryManager_AddItem;
            On.InventoryManager.GetItemQuantity += InventoryManager_GetItemQuantity;
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
            On.UpgradeButtonData.IsStoryUnlocked += UpgradeButtonData_IsStoryUnlocked;
            //update loops for Archipelago
            Courier.Events.PlayerController.OnUpdate += PlayerController_OnUpdate;
            On.InGameHud.OnGUI += InGameHud_OnGUI;
            //temp add
            On.PowerSeal.OnEnterRoom += PowerSeal_OnEnterRoom;
            On.DialogSequence.GetDialogList += DialogSequence_GetDialogList;
            On.LevelManager.OnLevelLoaded += LevelManager_onLevelLoaded;

            Console.WriteLine("Randomizer finished loading!");
        }
        
        public override void Initialize()
        {
            //I only want the generate seed/enter seed mod options available when not in the game.
            loadRandomizerFileForFileSlotButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            resetRandoSaveFileButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            //Also the AP buttons
            archipelagoHostButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            archipelagoPortButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            archipelagoNameButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            archipelagoPassButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
            archipelagoConnectButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;

            //Options I only want working while actually in the game
            windmillShurikenToggleButton.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && Manager<InventoryManager>.Instance.GetItemQuantity(EItems.WINDMILL_SHURIKEN) > 0);
            teleportToHqButton.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && randoStateManager.IsSafeTeleportState());
            teleportToNinjaVillage.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE && Manager<ProgressionManager>.Instance.HasCutscenePlayed("ElderAwardSeedCutscene") && randoStateManager.IsSafeTeleportState());
            seedNumButton.IsEnabled = () => (Manager<LevelManager>.Instance.GetCurrentLevelEnum() != ELevel.NONE);

            SceneManager.sceneLoaded += OnSceneLoadedRando;

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
            Console.WriteLine($"Starting dialogue{self.dialogID}");
            //Using this function to add some of my own dialog stuff to the game.
            if(randoStateManager.IsRandomizedFile && (self.dialogID == "RANDO_ITEM"))
            {
                Console.WriteLine("Trying some rando dialog stuff.");
                List<DialogInfo> dialogInfoList = new List<DialogInfo>();
                DialogInfo dialog = new DialogInfo();
                switch (self.dialogID)
                {
                    case "RANDO_ITEM":
                        dialog.text = $"You have received item: '{self.name}'";
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

            LocationRO randoItemCheck;

            if (itemId != EItems.TIME_SHARD) //killing the timeshard noise in the logs
            {
                Console.WriteLine($"Called InventoryManager_AddItem method. Looking to give x{quantity} amount of item '{itemId}'.");
                if (quantity == ItemsAndLocationsHandler.APQuantity)
                {
                    //We received this item from the AP server so grant it
                    orig(self, itemId, 1);
                    return;
                }
                if (ArchipelagoClient.HasConnected && randoStateManager.IsLocationRandomized(itemId, out randoItemCheck))
                {
                    ItemsAndLocationsHandler.SendLocationCheck(randoItemCheck);
                    if (randoStateManager.CurrentLocationToItemMapping[randoItemCheck].RecipientName == String.Empty)
                    {
                        //This isn't our item so we save and exit, otherwise the existing rando code can resolve it fine
                        Save.seedData = randomizerSaveMethod.GenerateSaveData();
                        return;
                    }
                }
            }

            //Wierd Ruxxtin logic stuff
            if(EItems.NONE.Equals(itemId))
            {
                Console.WriteLine("Looks like Ruxxtin has a timeshard.");
            }

            //Lets make sure that the item they are collecting is supposed to be randomized
            if (randoStateManager.IsRandomizedFile && !RandomizerStateManager.Instance.HasTempOverrideOnRandoItem(itemId) && randoStateManager.IsLocationRandomized(itemId, out randoItemCheck))
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
            LocationRO check;
            //Check if this is an Archipelago seed and whether it was checked in that state
            if (ArchipelagoClient.HasConnected)
            {
                try
                {
                    if (randoStateManager.IsLocationRandomized(self.item, out check))
                    {
                        Console.WriteLine($"Checking if {check.PrettyLocationName} has been checked.");
                        if (ArchipelagoClient.ServerData.CheckedLocations.Contains(ItemsAndLocationsHandler.LocationsLookup[check]))
                        {
                            Console.WriteLine("Returning true");
                            return true;
                        }
                    }
                    return orig(self);
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); return false; }
            }

            bool hasItem = false;
            //Check to make sure this is an item that was randomized and make sure we are not ignoring this specific trigger check
            if (randoStateManager.IsRandomizedFile && randoStateManager.IsLocationRandomized(self.item, out check) && !RandomizerConstants.GetSpecialTriggerNames().Contains(self.Owner.name))
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
                Console.WriteLine("HasItem check was not randomized. Doing vanilla checks.");
                Console.WriteLine($"Is randomized file : '{randoStateManager.IsRandomizedFile}' | Is location '{self.item}' randomized: '{randoStateManager.IsLocationRandomized(self.item, out check)}' | Not in the special triggers list: '{!RandomizerConstants.GetSpecialTriggerNames().Contains(self.Owner.name)}'|");
                return orig(self);
            }
            
        }
        
        int InventoryManager_GetItemQuantity(On.InventoryManager.orig_GetItemQuantity orig, InventoryManager self, EItems item)
        {
            //Just doing some logging here
            if (EItems.NONE.Equals(item))
            {
                Console.WriteLine($"INVENTORYMANAGER_GETITEMQUANTITY CALLED! Let's learn some stuff. Item: '{item}' | Quantity of said item: '{orig(self, item)}'");
            }
            //Manager<LevelManager>.Instance.onLevelLoaded
            return orig(self, item);
        }

        System.Collections.IEnumerator LevelManager_onLevelLoaded(On.LevelManager.orig_OnLevelLoaded orig, LevelManager self, Scene scene)
        {
            Console.WriteLine($"Scene '{scene.name}' loaded.");

            return orig(self, scene);
        }


        bool AwardNoteCutscene_ShouldPlay(On.AwardNoteCutscene.orig_ShouldPlay orig, AwardNoteCutscene self)
        {
            //Need to handle note cutscene triggers so they will play as long as I dont have the actual item it grants
            LocationRO noteCheck;
            if (randoStateManager.IsRandomizedFile && randoStateManager.IsLocationRandomized(self.noteToAward, out noteCheck)) //Double checking to prevent errors
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
            LocationRO cutsceneCheck;
            if (randoStateManager.IsRandomizedFile && RandomizerConstants.GetCutsceneMappings().ContainsKey(self.cutsceneId) && randoStateManager.IsLocationRandomized(RandomizerConstants.GetCutsceneMappings()[self.cutsceneId], out cutsceneCheck))
            {

                //Check to make sure this is a cutscene i am configured to check, then check to make sure I actually have the item that is mapped to it
                Console.WriteLine($"Rando cutscene magic ahoy! Handling rando cutscene '{self.cutsceneId}' | Linked Item: {RandomizerConstants.GetCutsceneMappings()[self.cutsceneId]} | Rando Item: {randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}");

                

                //Check to see if I have the item that is at this check.
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[cutsceneCheck].Item) >= 1)
                if(randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]))
                {
                    //Return true, this cutscene has "been played"
                    Console.WriteLine($"Have rando item '{randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}' for cutscene '{self.cutsceneId}'. Progress Manager on if cutscene has played: '{Manager<ProgressionManager>.Instance.HasCutscenePlayed(self.cutsceneId)}'. Returning that we have already seen cutscene.");
                    return self.mustHavePlayed == true;
                }
                else
                {
                    //Havent seen the cutscene yet. Play it so i can get the item!
                    Console.WriteLine($"Do not have rando item '{randoStateManager.CurrentLocationToItemMapping[cutsceneCheck]}' for cutscene '{self.cutsceneId}'. Progress Manager on if cutscene has played: '{Manager<ProgressionManager>.Instance.HasCutscenePlayed(self.cutsceneId)}'. Returning that we have not seen cutscene yet.");
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

            //This is probably a bad way to do this
            try
            {
                if (ArchipelagoData.LoadData(fileSlot))
                {
                    //The player is connected to an Archipelago server and trying to load a save file so check it's valid
                    Console.WriteLine($"Successfully loaded Archipelago seed {fileSlot}");
                }
                else if (ArchipelagoClient.Authenticated && Manager<SaveManager>.Instance.GetCurrentSaveGameSlot().SecondsPlayed <= 100)
                {
                    //Hopefully this ensures this is a clean rando slot so the player doesn't just connect with an invalid slot
                    randoStateManager.AddSeed(ArchipelagoClient.ServerData.StartNewSeed(fileSlot));
                    randoStateManager.CurrentLocationToItemMapping = ArchipelagoClient.ServerData.LocationToItemMapping;
                }
                else
                {
                    //There's a valid seed and mapping available and Archipelago isn't involved
                    randoStateManager.CurrentLocationToItemMapping = ItemRandomizerUtil.ParseLocationToItemMappings(randoStateManager.GetSeedForFileSlot(fileSlot));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                orig(self, slotIndex);
            }
            //Generate the mappings based on the seed for the game if a seed was generated.
            if (randoStateManager.HasSeedForFileSlot(fileSlot) || ArchipelagoClient.HasConnected)
            {
                Console.WriteLine($"Seed exists for file slot {fileSlot}. Generating mappings.");
                
                //Load mappings
                randoStateManager.CurrentLocationDialogtoRandomDialogMapping = DialogChanger.GenerateDialogMappingforItems();

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
                Console.WriteLine($"Seed Info: {randoStateManager.GetSeedForFileSlot(fileSlot).Seed}");
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
            LocationRO necroLocation;
            if(randoStateManager.IsRandomizedFile && randoStateManager.IsLocationRandomized(EItems.NECROPHOBIC_WORKER, out necroLocation))
            {
                //check to see if we already have the item at Necro check
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())].Item) <= 0 && !Manager<DemoManager>.Instance.demoMode)
                if (!randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[necroLocation]) && !Manager<DemoManager>.Instance.demoMode)
                {
                    //Run the cutscene if we dont
                    Console.WriteLine($"Have not received item '{randoStateManager.CurrentLocationToItemMapping[necroLocation]}' from Necro check. Playing cutscene.");
                    self.necrophobicWorkerCutscene.Play();
                }
                //if (Manager<InventoryManager>.Instance.GetItemQuantity(randoStateManager.CurrentLocationToItemMapping[new LocationRO(EItems.NECROPHOBIC_WORKER.ToString())].Item) >= 1 || Manager<DemoManager>.Instance.demoMode)
                if (randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).CollectedItems.Contains(randoStateManager.CurrentLocationToItemMapping[necroLocation]) || Manager<DemoManager>.Instance.demoMode)
                {
                    //set necro inactive if we do
                    Console.WriteLine($"Already have item '{randoStateManager.CurrentLocationToItemMapping[necroLocation]}' from Necro check. Will not play cutscene.");
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

        bool UpgradeButtonData_IsStoryUnlocked(On.UpgradeButtonData.orig_IsStoryUnlocked orig, UpgradeButtonData self)
        {
            bool isUnlocked;

            //Checking if this particular upgrade is the glide attack
            if(EShopUpgradeID.GLIDE_ATTACK.Equals(self.upgradeID))
            {
                //Unlock the glide attack (no need to keep it hidden, player can just buy it whenever they want.
                isUnlocked = true;
            }
            else
            {
                isUnlocked = orig(self);
            }

            //I think there is where I can catch things like checks for the wingsuit attack upgrade.
            Console.WriteLine($"Checking upgrade '{self.upgradeID}'. Is story unlocked: {isUnlocked}");

            return isUnlocked;
        }

        ///On submit of rando file location
        bool OnEnterFileSlot(string fileSlot)
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

            //Load encoded seed information
            string encodedSeedInfo = ItemRandomizerUtil.LoadMappingsFromFile(slot);
            Console.WriteLine($"File reading complete. Received the following encoded seed info: '{encodedSeedInfo}'");
            string decodedSeedInfo = ItemRandomizerUtil.DecryptSeedInfo(encodedSeedInfo);
            Console.WriteLine($"Decryption complete. Received the following seed info: '{decodedSeedInfo}'");
            SeedRO seedRO = ItemRandomizerUtil.ParseSeed(slot, decodedSeedInfo);

            randoStateManager.AddSeed(seedRO);

            //Save
            Save.seedData = randomizerSaveMethod.GenerateSaveData();

            return true;
        }

        bool OnRandoFileResetConfirmation(string answer)
        {
            Console.WriteLine($"In Method: OnResetRandoFileSlot. Provided value: '{answer}'");
            
            if(!"y".Equals(answer.ToLowerInvariant()))
            {
                return true;
            }

            ArchipelagoData.ClearData();
            randoStateManager.ResetRandomizerState();
            string path = Application.persistentDataPath + "/SaveGame.txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(RandomizerConstants.SAVE_FILE_STRING);
            }
            
            Console.WriteLine("Save file written. Now loading file.");
            Manager<SaveManager>.Instance.LoadSaveGame();
            //Delete the existing save file selection ui since it really wants to hold on to the previous saves data.
            GameObject.Destroy(Manager<UIManager>.Instance.GetView<SaveGameSelectionScreen>().gameObject);
            //Reinit the save file selection ui.
            SaveGameSelectionScreen selectionScreen = Manager<UIManager>.Instance.ShowView<SaveGameSelectionScreen>(EScreenLayers.MAIN, null, false, AnimatorUpdateMode.Normal);
            selectionScreen.GoOffscreenInstant();

            return true;
        }

        public static void OnToggleWindmillShuriken()
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

        bool OnSelectArchipelagoHost(string answer)
        {
            Console.WriteLine($"Adding Archipelago host information: {answer}");
            if (answer == null) return true;
            if (ArchipelagoClient.ServerData == null) ArchipelagoClient.ServerData = new ArchipelagoData();
            var uri = answer;
            if (answer.Contains(' '))
            {
                var splits = answer.Split(' ');
                uri = String.Join(" ", splits.ToArray());
            }
            ArchipelagoClient.ServerData.Uri = uri;
            return true;
        }

        bool OnSelectArchipelagoPort(string answer)
        {
            Console.WriteLine($"Adding Archipelago port information: {answer}");
            if (answer == null) return true;
            if (ArchipelagoClient.ServerData == null) ArchipelagoClient.ServerData = new ArchipelagoData();
            var port = 38281;
            int.TryParse(answer, out port);
            ArchipelagoClient.ServerData.Port = port;
            return true;
        }

        bool OnSelectArchipelagoName(string answer)
        {
            Console.WriteLine($"Adding Archipelago slot name information: {answer}");
            if (answer == null) return true;
            if (ArchipelagoClient.ServerData == null) ArchipelagoClient.ServerData = new ArchipelagoData();
            ArchipelagoClient.ServerData.SlotName = answer;
            return true;
        }

        bool OnSelectArchipelagoPass(string answer)
        {
            Console.WriteLine($"Adding Archipelago password information: {answer}");
            if (answer == null) return true;
            if (ArchipelagoClient.ServerData == null) ArchipelagoClient.ServerData = new ArchipelagoData();
            ArchipelagoClient.ServerData.Password = answer;
            return true;
        }

        void OnSelectArchipelagoConnect()
        {
            if (ArchipelagoClient.ServerData == null) return;
            if (ArchipelagoClient.ServerData.SlotName == null) return;
            if (ArchipelagoClient.ServerData.Uri == null) ArchipelagoClient.ServerData.Uri = "archipelago.gg";
            if (ArchipelagoClient.ServerData.Port == 0) ArchipelagoClient.ServerData.Port = 38281;

            ArchipelagoClient.ConnectAsync();
        }

        /// <summary>
        /// Delegate function for getting rando item. This can be used by IL hooks that need to make this call later.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private EItems GetRandoItemByItem(EItems item)
        {
            LocationRO ruxxAmuletLocation;
            
            if(randoStateManager.IsLocationRandomized(item, out ruxxAmuletLocation))
            {
                Console.WriteLine($"IL Wackiness -- Checking for Item '{item}' | Rando item to return '{randoStateManager.CurrentLocationToItemMapping[ruxxAmuletLocation]}'");

                EItems randoItem = randoStateManager.CurrentLocationToItemMapping[ruxxAmuletLocation].Item;
                
                if(EItems.TIME_SHARD.Equals(randoItem))
                {
                    /* Having a lot of problems with timeshards and the ruxxtin check due to it having some checks behind the scenes.
                     * What I am trying is to change the item to the NONE value since that is expected to have no quantity. This will trick the cutscene into playing correctly the first time.
                     * Checks after the first time rely on the collected items list so it shouldn't have any impact...
                     */
                    randoItem = EItems.NONE;
                }
                return randoItem;
            }
            else
            {
                return item;
            }
            
        }

        private string GetCurrentSeedNum()
        {
            string seedNum = "Unknown";

            if(randoStateManager != null && randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).Seed > 0)
            {
                seedNum = randoStateManager.GetSeedForFileSlot(randoStateManager.CurrentFileSlot).Seed.ToString();
            }
            if (ArchipelagoClient.HasConnected)
            {
                seedNum = ArchipelagoClient.ServerData.SeedName;
            }

            return seedNum;
        }

        private void OnSceneLoadedRando(Scene scene, LoadSceneMode mode)
        {
            Console.WriteLine($"Scene loaded: '{scene.name}'");
        }

        private void PlayerController_OnUpdate(PlayerController controller)
        {
            if (!ArchipelagoClient.HasConnected) return;
            if (Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE || !randoStateManager.IsSafeTeleportState())
            {
                return;
            }
            float updateTime = 1.0f;
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateTime)
            {
                updateTimer = 0;
                ArchipelagoClient.UpdateArchipelagoState();
            }
        }

        private void InGameHud_OnGUI(On.InGameHud.orig_OnGUI orig, InGameHud self)
        {
            orig(self);
            if (apMenu8 == null)
            {
                apMenu8 = UnityEngine.Object.Instantiate(self.hud_8.coinCount, self.hud_8.gameObject.transform);
                apMenu16 = UnityEngine.Object.Instantiate(self.hud_16.coinCount, self.hud_16.gameObject.transform);
                apMenu8.transform.Translate(0f, -110f, 0f);
                apMenu16.transform.Translate(0f, -110f, 0f);
                apMenu16.fontSize = apMenu8.fontSize = 3.8f;
                apMenu16.alignment = apMenu8.alignment = TextAlignmentOptions.TopRight;
                apMenu16.enableWordWrapping = apMenu8.enableWordWrapping = false;
                apMenu16.color = apMenu8.color = Color.white;
            }
            apMenu16.text = apMenu8.text = ArchipelagoClient.UpdateMenuText();
        }
    }
}
