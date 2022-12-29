using System.Collections.Generic;
using MessengerRando.RO;

namespace MessengerRando.Archipelago
{
    public class ArchipelagoData
    {
        public string Uri;
        public int Port;
        public string SlotName;
        public string Password;
        public int Index = 0;
        public string SeedName;
        public Dictionary<string, object> SlotData;
        public readonly List<LocationRO> CheckedLocations = new List<LocationRO>();
    }
}