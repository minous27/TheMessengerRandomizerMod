

namespace MessengerRando.RO
{
    public class LocationRO
    {
        public EItems LocationName { get; private set; }
        public EItems[] AdditionalRequiredItemsForCheck { get; private set; }
        public bool IsWingsuitRequired { get; private set; }
        public bool IsRopeDartRequired { get; private set; }
        public bool IsEitherWingsuitOrRopeDartRequired { get; private set; }
        public bool IsNinjaTabiRequired { get; private set; }

        public LocationRO(EItems name, EItems[] requiredItemsToCheck, bool isWingsuitRequired, bool isRopeDartRequired, bool isNinjaTabiRequired, bool isEitherWingsuitOrRopeDartRequired = false)
        {
            this.LocationName = name;
            this.AdditionalRequiredItemsForCheck = requiredItemsToCheck;
            this.IsWingsuitRequired = isWingsuitRequired;
            this.IsRopeDartRequired = isRopeDartRequired;
            this.IsEitherWingsuitOrRopeDartRequired = isEitherWingsuitOrRopeDartRequired;
            this.IsNinjaTabiRequired = isNinjaTabiRequired;
        }

        public LocationRO(EItems name) : this(name, new EItems[] { }, false, false, false){}

        public override bool Equals(object obj)
        {
            return obj is LocationRO rO &&
                   LocationName == rO.LocationName;
        }

        public override int GetHashCode()
        {
            return 771690509 + LocationName.GetHashCode();
        }
    }
}
