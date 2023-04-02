using System;
using System.Collections.Generic;
using System.Text;
using MessengerRando.Utils;

namespace MessengerRando.RO
{
    /// <summary>
    /// Class that represents the seed the player is playing.
    /// </summary>
    [Serializable]
    public struct SeedRO
    {
        public int FileSlot { get; set; }
        // Type of seed so we know what logic to run against it.
        public SeedType SeedType { get; set; }
        //seed number
        public int Seed { get; set; }
        // Settings
        public Dictionary<SettingType, SettingValue> Settings{ get; set; }
        //Collected Items this seed
        public List<RandoItemRO> CollectedItems { set; get; }
        //Base64 encrypted string representing all the mappings for this seed
        public string MappingInfo { get; set; }

        public SeedRO(int fileSlot, SeedType seedType, int seed, Dictionary<SettingType, SettingValue> settings, List<RandoItemRO> collectedItems, string mappingString)
        {
            FileSlot = fileSlot;
            SeedType = seedType;
            Seed = seed;
            
            if(settings == null)
            {
                settings = new Dictionary<SettingType, SettingValue>();
            }
            Settings = settings;

            CollectedItems = collectedItems == null ? new List<RandoItemRO>() : collectedItems;

            MappingInfo = mappingString;
        }

        public override string ToString()
        {
            //Set up settings string
            StringBuilder settingsSB = new StringBuilder();
            settingsSB.Append("Settings:'");
            foreach(SettingType key in Settings.Keys)
            {
                settingsSB.Append($"{key}={Settings[key]}");
                settingsSB.Append("&");
            }
            settingsSB.Append("'");

            //Set up collected items string
            StringBuilder collectedItemsSB = new StringBuilder();
            collectedItemsSB.Append("CollectedItems:'");
            foreach(RandoItemRO item in CollectedItems)
            {
                collectedItemsSB.Append($"{item}");
                collectedItemsSB.Append("&");
            }
            settingsSB.Append("'");

            return $"{FileSlot}|{SeedType}|{Seed}|{settingsSB.ToString()}|{collectedItemsSB.ToString()}|";
        }

        public override bool Equals(object obj)
        {
            return obj is SeedRO rO &&
                   SeedType == rO.SeedType &&
                   Seed == rO.Seed &&
                   EqualityComparer<Dictionary<SettingType, SettingValue>>.Default.Equals(Settings, rO.Settings) &&
                   EqualityComparer<List<RandoItemRO>>.Default.Equals(CollectedItems, rO.CollectedItems);
        }

        public override int GetHashCode()
        {
            var hashCode = -514544316;
            hashCode = hashCode * -1521134295 + SeedType.GetHashCode();
            hashCode = hashCode * -1521134295 + Seed.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<SettingType, SettingValue>>.Default.GetHashCode(Settings);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<RandoItemRO>>.Default.GetHashCode(CollectedItems);
            return hashCode;
        }
    }
}
