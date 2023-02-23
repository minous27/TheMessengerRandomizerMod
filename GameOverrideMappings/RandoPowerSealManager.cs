using System;
using System.Collections.Generic;
using MessengerRando.Archipelago;
using Object = UnityEngine.Object;

namespace MessengerRando.GameOverrideMappings
{
    public class RandoPowerSealManager
    {
        private int amountPowerSealsCollected;
        private readonly int requiredPowerSeals;

        public RandoPowerSealManager(int requiredPowerSeals)
        {
            this.requiredPowerSeals = Manager<ProgressionManager>.Instance.powerSealTotal = requiredPowerSeals;
            amountPowerSealsCollected = ArchipelagoClient.ServerData.PowerSealsCollected;
        }

        public void AddPowerSeal() => ArchipelagoClient.ServerData.PowerSealsCollected = ++amountPowerSealsCollected;

        public void ShopChestSetState(On.ShopChest.orig_SetState orig, ShopChest shopChest)
        {
            // if (CanOpenChest() && !Manager<ProgressionManager>.Instance.HasCutscenePlayed<ShopChestOpenCutscene>())
            //     ArchipelagoClient.UpdateClientStatus(ArchipelagoClientState.ClientGoal);
            
            //no idea why but this block of code causes the game to freeze when the chest can't be opened.
            //Haven't tracked down the failing event so still calling the original code which will reward the windmill shuriken.
            //
            // if (!CanOpenChest())
            //     shopChest.animator.SetTrigger("ClosedInstant");
            // else if (CanOpenChest() && !ProgressionManager.HasCutscenePlayed<ShopChestOpenCutscene>())
            //     shopChest.animator.SetTrigger("ClosedReadyInstant");
            // else if (ProgressionManager.useWindmillShuriken)
            //     shopChest.animator.SetTrigger("OpenedShurikenInstant");
            // else
            //     shopChest.animator.SetTrigger("OpenedWindmillInstant");
            //
            orig(shopChest);
        }

        public void OnShopChestOpen(On.ShopChestOpenCutscene.orig_OnChestOpened orig, ShopChestOpenCutscene self)
        {
            if (new List<string>{"open_shop_chest"}.Contains(RandomizerStateManager.Instance.Goal))
            {
                //going to attempt to teleport the player to the ending sequence when they open the chest
                try
                {
                    Object.FindObjectOfType<Shop>().LeaveToCurrentLevel();
                    RandoLevelManager.TeleportToMusicBox();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                self.EndCutScene();
            }
            else orig(self);
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