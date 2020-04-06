using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Mod.Courier;
using Mod.Courier.Module;
using Mod.Courier.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Mod.Courier.UI.TextEntryButtonInfo;


namespace MessengerRando 
{
    public class RandomizedItemInserter : CourierModule
    {
        private int seed = 0;
        private Dictionary<EItems, EItems> locationToItemMapping;
        

        TextEntryButtonInfo generateSeedButton;

        public override void Load()
        {
            Console.WriteLine("Randomizer loaded and ready to try things!");
            //Start the randomizer util initializations
            ItemRandomizerUtil.Load();
            //Generate the seed for this rando
            seed = ItemRandomizerUtil.GenerateSeed();
            //Generate the randomized mappings
            locationToItemMapping = ItemRandomizerUtil.GenerateRandomizedMappings(seed);
            //Add generate mod option button
            generateSeedButton = Courier.UI.RegisterTextEntryModOptionButton(() => "Generate Random Seed", OnEnterRandoFileName, 10, () => "Please provide rando file name.", null);

            On.InventoryManager.AddItem += InventoryManager_AddItem;
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
            if (locationToItemMapping.ContainsKey(randoItemId))
            {
                //Based on the item that is attempting to be added, determine what SHOULD be added instead
                randoItemId = locationToItemMapping[itemId];
                randoQuantity = 1;
            }

            //Call original add with items
            orig(self, randoItemId, randoQuantity);
        }

        //On submit of rando file name
        bool OnEnterRandoFileName(string fileName)
        {
            Console.WriteLine($"File name received: {fileName}");

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

        //On submit of rando file location
        bool OnEnterRandoFileSlot(string fileSlot)
        {
            Console.WriteLine($"Received file slot number: {fileSlot}");

            int slot = Convert.ToInt32(fileSlot);

            //Check to make sure an apporiate save slot was chosen.
            if (slot < 1 || slot > 3)
            {
                Console.WriteLine($"User provided an invalid save slot number {slot}");
                return false;
            }

            //Generate a seed
            Console.WriteLine("Generating seed...");
            this.seed = ItemRandomizerUtil.GenerateSeed();
            Console.WriteLine($"Seed generated: '{this.seed}'");

            //TODO Would start creating the save file

            return true;
        }
    }
}
