using System.Collections.Generic;

namespace MessengerRando.GameOverrideMappings
{
    public class RandoBossManager
    {
        private readonly List<string> defeatedBosses;
        private Dictionary<string, string> origToNewBoss;

        public RandoBossManager(Dictionary<string, string> bossMapping)
        {
            Manager<ProgressionManager>.Instance.bossesDefeated =
                Manager<ProgressionManager>.Instance.allTimeBossesDefeated = defeatedBosses = new List<string>();
            origToNewBoss = bossMapping;
        }

        public bool HasBossDefeated(string bossName)
        {
            return defeatedBosses.Contains(bossName);
        }

        public void SetBossAsDefeated(string bossName)
        {
            defeatedBosses.Add(bossName);
        }
    }
}