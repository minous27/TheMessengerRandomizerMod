using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessengerRando.RO
{
    /// <summary>
    /// This RO represents a hypothetical player in the game. It is used to simulate play for validations/testing.
    /// </summary>
    public struct SamplePlayerRO
    {
        public bool HasRopeDart { get; set; }
        public bool HasWingsuit { get; set; }
        public bool HasNinjaTabis { get; set; }

        public int NoteCount { get; set; }
        
        //Currently contains all other items and notes
        public List<RandoItemRO> AdditionalItems { get; set; }

        public SamplePlayerRO(bool hasRopeDart, bool hasWingsuit, bool hasNinjaTabis, int noteCount, List<RandoItemRO> additionalItems)
        {
            HasRopeDart = hasRopeDart;
            HasWingsuit = hasWingsuit;
            HasNinjaTabis = hasNinjaTabis;
            NoteCount = noteCount;
            AdditionalItems = additionalItems;
        }
    }
}
