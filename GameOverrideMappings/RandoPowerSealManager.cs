using Archipelago.MultiClient.Net.Enums;
using MessengerRando.Archipelago;

namespace MessengerRando.GameOverrideMappings
{
    public class RandoPowerSealManager
    {
        private int totalPowerSealsCollected;
        private readonly int requiredPowerSeals;

        public RandoPowerSealManager(int requiredPowerSeals)
        {
            this.requiredPowerSeals = requiredPowerSeals;
        }

        public void AddPowerSeal()
        {
            ++totalPowerSealsCollected;
        }

        public void ShopChestSetState(ShopChest shopChest)
        {
            if (!CanOpenChest())
                shopChest.animator.SetTrigger("ClosedInstant");
            // for some reason this line is hardcoded in the decompiled code so had to essentially override the entire
            // function
            else if (CanOpenChest() && !Manager<ProgressionManager>.Instance.HasCutscenePlayed<ShopChestOpenCutscene>())
            {
                shopChest.animator.SetTrigger("ClosedReadyInstant");
                if (RandomizerStateManager.Instance.Goal.Equals("Chest") && ArchipelagoClient.Authenticated)
                    ArchipelagoClient.UpdateClientStatus(ArchipelagoClientState.ClientGoal);
            }
            else if (Manager<ProgressionManager>.Instance.useWindmillShuriken)
                shopChest.animator.SetTrigger("OpenedShurikenInstant");
            else
                shopChest.animator.SetTrigger("OpenedWindmillInstant");
        }

        public int TotalPowerSeals() => totalPowerSealsCollected;
        public bool CanOpenChest() => totalPowerSealsCollected >= requiredPowerSeals;
    }
}