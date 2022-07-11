
namespace MessengerRando.RO
{
    public class LocationRO
    {
        public string LocationName { get; private set; }
        public string PrettyLocationName { get; private set; } //Can be same as LocationName, used for logging and spoiler log
        public EItems[] AdditionalRequiredItemsForCheck { get; private set; }
        public bool IsWingsuitRequired { get; private set; }
        public bool IsRopeDartRequired { get; private set; }
        public bool IsEitherWingsuitOrRopeDartRequired { get; private set; }
        public bool IsNinjaTabiRequired { get; private set; }

        public LocationRO(string name, string prettyName, EItems[] requiredItemsToCheck, bool isWingsuitRequired, bool isRopeDartRequired, bool isNinjaTabiRequired, bool isEitherWingsuitOrRopeDartRequired = false)
        {
            LocationName = name;
            PrettyLocationName = prettyName;
            AdditionalRequiredItemsForCheck = requiredItemsToCheck;
            IsWingsuitRequired = isWingsuitRequired;
            IsRopeDartRequired = isRopeDartRequired;
            IsEitherWingsuitOrRopeDartRequired = isEitherWingsuitOrRopeDartRequired;
            IsNinjaTabiRequired = isNinjaTabiRequired;
        }

        public LocationRO(string name) : this(name, name, new EItems[] { }, false, false, false){}

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
