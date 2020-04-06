using System;
using System.Collections.Generic;
using System.Reflection;
using Mod.Courier;
using Mod.Courier.Helpers;
using Mod.Courier.Module;
using Mod.Courier.UI;
using UnityEngine;


namespace MessengerRando 
{
    public class RandomizedItemInserter : CourierModule
    {
        private int seed = 0;
        private Dictionary<EItems, EItems> locationToItemMapping;
        

        SubMenuButtonInfo generateSeedButton;

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
            generateSeedButton = Courier.UI.RegisterSubMenuModOptionButton(() => "Generate Seed", GenerateSeed);

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

        void GenerateSeed()
        {
            Console.WriteLine("Generating seed...");
            this.seed = ItemRandomizerUtil.GenerateSeed();
            Console.WriteLine($"Seed generated: '{this.seed}'");
        }
    }
}
