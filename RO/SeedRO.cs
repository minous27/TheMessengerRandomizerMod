using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessengerRando.Utils;

namespace MessengerRando.RO
{
    public struct SeedRO
    {
        // Type of seed so we know what logic to run against it.
        public SeedType SeedType { get; }
        //seed number
        public int Seed { get; }

        public SeedRO(SeedType seedType, int seed)
        {
            SeedType = seedType;
            Seed = seed;
        }

        public override string ToString () => $"{SeedType}|{Seed}";

        public override bool Equals(object obj)
        {
            return obj is SeedRO rO &&
                   SeedType == rO.SeedType &&
                   Seed == rO.Seed;
        }

        public override int GetHashCode()
        {
            var hashCode = 637891472;
            hashCode = hashCode * -1521134295 + SeedType.GetHashCode();
            hashCode = hashCode * -1521134295 + Seed.GetHashCode();
            return hashCode;
        }
    }
}
