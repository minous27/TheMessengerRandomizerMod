using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessengerRando.RO
{
    public struct RandoItemRO
    {
        //some kind of unique identifier
        public string Name { get; }
        //the in game item they represent
        public EItems Item { get; }
        //amount to give
        public int Quantity { get; }

        public RandoItemRO(string name, EItems item, int quantity = 1)
        {
            Name = name;
            Item = item;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Name} - {Item}";
        }

        public override bool Equals(object obj)
        {
            return obj is RandoItemRO rO &&
                   Name == rO.Name &&
                   Item == rO.Item;
        }

        public override int GetHashCode()
        {
            var hashCode = 1065740796;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Item.GetHashCode();
            return hashCode;
        }
    }
}
