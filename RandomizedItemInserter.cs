using System;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.UI;
using static Mod.Courier.UI.TextEntryButtonInfo;


namespace MessengerRando 
{
    public class RandomizedItemInserter : CourierModule
    {
        private const string RANDO_OPTION_KEY = "minous27RandoSeeds";

        private RandomizerStateManager randoStateManager;       

        TextEntryButtonInfo generateSeedButton;

        public override void Load()
        {
            Console.WriteLine("Randomizer loaded and ready to try things!");
           
            //Start the randomizer util initializations
            ItemRandomizerUtil.Load();
            //Initialize the randomizer state manager
            RandomizerStateManager.Initialize();
            randoStateManager = RandomizerStateManager.Instance;

            //Add generate mod option button
            generateSeedButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Generate Random Seed", OnEnterRandoFileSlot, 1, () => "Which save slot would you like to start a rando seed?", () => "1", CharsetFlags.Number);
            generateSeedButton.SaveMethod = new RandomizerSaveMethod(RANDO_OPTION_KEY);

            //Plug in my code :3
            On.InventoryManager.AddItem += InventoryManager_AddItem;
            On.HasItem.IsTrue += HasItem_IsTrue;
            On.AwardNoteCutscene.ShouldPlay += AwardNoteCutscene_ShouldPlay;
            //On.CutsceneHasPlayed.IsTrue += CutsceneHasPlayed_IsTrue;
            On.SaveGameSelectionScreen.OnLoadGame += SaveGameSelectionScreen_OnLoadGame;
            On.SaveGameSelectionScreen.OnNewGame += SaveGameSelectionScreen_OnNewGame;
            
            Console.WriteLine("Randomizer finished loading!");
        }

        public override void Initialize()
        {
            //I only want the generate seed mod option available when not in the game.
            generateSeedButton.IsEnabled = () => Manager<LevelManager>.Instance.GetCurrentLevelEnum() == ELevel.NONE;
        }

        void InventoryManager_AddItem(On.InventoryManager.orig_AddItem orig, InventoryManager self, EItems itemId, int quantity)
        {
            //Currently defaulting rando values in case this is not a randomized item like pickups
            EItems randoItemId = itemId;
            int randoQuantity = quantity;

            //Lets make sure that the item they are collecting is supposed to be randomized
            if (randoStateManager.CurrentLocationToItemMapping.ContainsKey(randoItemId))
            {
                //Based on the item that is attempting to be added, determine what SHOULD be added instead
                randoItemId = randoStateManager.CurrentLocationToItemMapping[itemId];
                randoQuantity = 1;
            }

            //Call original add with items
            orig(self, randoItemId, randoQuantity);
        }

        bool HasItem_IsTrue(On.HasItem.orig_IsTrue orig, HasItem self)
        {
            bool hasItem = false;
            //Check to make sure this is an item that was randomized and make sure we are not ignoring this specific trigger check
            if (ItemRandomizerUtil.RandomizableLocations.Contains(self.item) && !ItemRandomizerUtil.TriggersToIgnoreRandoItemLogic.Contains(self.Owner.name))
            {
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
            if (randoStateManager.CurrentLocationToItemMapping.ContainsKey(self.noteToAward)) //Double checking to prevent errors
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
            

            //Check to make sure this is a cutscene i am configured to check, then check to make sure I actually have the item that is mapped to it
            if(ItemRandomizerUtil.CutsceneMappings.ContainsKey(self.cutsceneId) && Manager<InventoryManager>.Instance.GetItemQuantity(ItemRandomizerUtil.CutsceneMappings[self.cutsceneId]) >= 1)
            {
                //Return true, this cutscene has "been played"
                return true;
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

        //On submit of rando file location
        bool OnEnterRandoFileSlot(string fileSlot)
        {
            Console.WriteLine($"Received file slot number: {fileSlot}");

            int randoSaveSlot = Convert.ToInt32(fileSlot);

            //Check to make sure an apporiate save slot was chosen.
            if (randoSaveSlot < 1 || randoSaveSlot > 3)
            {
                Console.WriteLine($"User provided an invalid save slot number {randoSaveSlot}");
                return false;
            }

            //Generate a seed
            Console.WriteLine("Generating seed...");
            int seed = ItemRandomizerUtil.GenerateSeed();
            Console.WriteLine($"Seed generated: '{seed}'");

            //Save this seed into the state
            randoStateManager.AddSeed(randoSaveSlot, seed);

            return true;
        }
    }
}
