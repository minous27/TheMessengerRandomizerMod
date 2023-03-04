using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MessengerRando.GameOverrideMappings
{
    public class RandoLevelManager
    {
        public static bool Teleporting;
        public static void SkipMusicBox()
        {
            if (Teleporting)
            {
                Teleporting = false;
                return;
            }
            Manager<AudioManager>.Instance.StopMusic();
            var playerPosition = RandomizerStateManager.Instance.SkipMusicBox
                ? new Vector2(125, 40)
                : new Vector2(-428, -55);

            var playerDimension = RandomizerStateManager.Instance.SkipMusicBox ? EBits.BITS_8 : EBits.BITS_16;

            TeleportInArea(ELevel.Level_11_B_MusicBox, playerPosition, playerDimension);
        }

        public static void TeleportInArea(ELevel area, Vector2 position, EBits dimension)
        {
            if (Teleporting)
            {
                Teleporting = false;
                return;
            }
            Manager<AudioManager>.Instance.StopMusic();
            Manager<ProgressionManager>.Instance.checkpointSaveInfo.loadedLevelPlayerPosition = position;
            LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(area + "_Build",
                false, true, LoadSceneMode.Single,
                ELevelEntranceID.NONE, dimension);
            Teleporting = true;
            Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
        }

        public static void ChangeRoom(On.Level.orig_ChangeRoom orig, Level self,
            ScreenEdge newRoomLeftEdge, ScreenEdge newRoomRightEdge,
            ScreenEdge newRoomBottomEdge, ScreenEdge newRoomTopEdge,
            bool teleportedInRoom)
        {
            string GetRoomKey()
            {
                return newRoomLeftEdge.edgeIdX + newRoomRightEdge.edgeIdX
                                               + newRoomBottomEdge.edgeIdY + newRoomTopEdge.edgeIdY;
            }

            if (RandomizerStateManager.Instance.SkipMusicBox && self.CurrentRoom.roomKey == "-436-404-60-44")
            {
                // newRoomLeftEdge.edgeIdX = 
            }
        }
    }
}