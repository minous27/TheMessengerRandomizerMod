using UnityEngine;
using UnityEngine.SceneManagement;

namespace MessengerRando.GameOverrideMappings
{
    public class RandoLevelManager
    {
        public static void TeleportToMusicBox()
        {
            Manager<AudioManager>.Instance.StopMusic();
            var playerPosition = RandomizerStateManager.Instance.SkipMusicBox
                ? new Vector3(125f, 40f)
                : new Vector3(-334.5f, -69f);

            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = playerPosition;
            LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(ELevel.Level_Ending + "_Build",
                true, false, LoadSceneMode.Single, ELevelEntranceID.ENTRANCE_A,
                EBits.BITS_8);
            Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
        }
    }
}