
namespace MessengerRando.Utils
{
    /// <summary>
    /// Enum that describes if the seed is logical or not.
    /// </summary>
    public enum SeedType
    {
        None, //No type yet set (signifies a seed yet to be set up)
        No_Logic, //Seed that does no logical checks
        Logic, //Seed that uses logic engine
        Archipelago //Seed was generated using Archipelago
    }
}
