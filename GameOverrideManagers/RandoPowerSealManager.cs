using System;
using System.Collections.Generic;
using MessengerRando.Archipelago;
using Object = UnityEngine.Object;

namespace MessengerRando.GameOverrideManagers
{
    public class RandoPowerSealManager
    {
        public static readonly List<string> Goals = new List<string> { "power_seal_hunt" };
        private int amountPowerSealsCollected;
        private readonly int requiredPowerSeals;

        public RandoPowerSealManager(int requiredPowerSeals)
        {
            this.requiredPowerSeals = Manager<ProgressionManager>.Instance.powerSealTotal = requiredPowerSeals;
            amountPowerSealsCollected = ArchipelagoClient.ServerData.PowerSealsCollected;
        }

        public void AddPowerSeal() => ArchipelagoClient.ServerData.PowerSealsCollected = ++amountPowerSealsCollected;


        public void OnShopChestOpen(On.ShopChestOpenCutscene.orig_OnChestOpened orig, ShopChestOpenCutscene self)
        {
            if (Goals.Contains(RandomizerStateManager.Instance.Goal) &&
                !Manager<LevelManager>.Instance.GetCurrentLevelEnum().Equals(ELevel.NONE))
            {
                //going to attempt to teleport the player to the ending sequence when they open the chest
                OnShopChestOpen();
                self.EndCutScene();
            }
            else orig(self);
        }

        public void OnShopChestOpen(On.ShopChestChangeShurikenCutscene.orig_Play orig, ShopChestChangeShurikenCutscene self)
        {
            if (Goals.Contains(RandomizerStateManager.Instance.Goal)
                && RandomizerStateManager.Instance.IsSafeTeleportState())
            {
                OnShopChestOpen();
                self.EndCutScene();
            }
            else orig(self);
        }

        private void OnShopChestOpen()
        {
            try
            {
                Object.FindObjectOfType<Shop>().LeaveToCurrentLevel();
                RandoLevelManager.SkipMusicBox();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Assigns our total power seal count to the game and then returns the value. Unsure if the assignment is safe
        /// here, but trying it so it'll show the required count in the dialog.
        /// </summary>
        /// <returns></returns>
        public int AmountPowerSealsCollected() => amountPowerSealsCollected;

        public bool CanOpenChest() => amountPowerSealsCollected >= requiredPowerSeals;
    }
}