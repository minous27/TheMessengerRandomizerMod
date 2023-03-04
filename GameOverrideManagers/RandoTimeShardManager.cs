using System.Collections.Generic;
using MessengerRando.RO;

namespace MessengerRando.GameOverrideManagers
{
    public class RandoTimeShardManager
    {


        public struct MegaShard
        {
            public readonly ELevel ShardRegion;
            public readonly string RoomKey;

            public MegaShard(ELevel area, string key)
            {
                ShardRegion = area;
                RoomKey = key;
            }
        }
        
        public static readonly Dictionary<MegaShard, LocationRO> MegaShardLookup = new Dictionary<MegaShard, LocationRO>
        {
            { new MegaShard(ELevel.Level_01_NinjaVillage, ""), new LocationRO("Ninja Village Time Shard")}
        };
    }
}