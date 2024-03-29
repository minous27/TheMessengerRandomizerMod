﻿using System;
using System.Collections.Generic;

namespace MessengerRando.RO
{
    /// <summary>
    /// Class that represents an item that has been randomly placed within the game.
    /// </summary>
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
            return $"{Name}-{Item}-{Quantity}";
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

        /// <summary>
        /// Expect three values in format 'name-item-quantity'.
        /// </summary>
        /// <param name="itemToParse"></param>
        /// <returns>RandoItemRO with the values passed in.</returns>
        public static RandoItemRO ParseString(string itemToParse)
        {

            if(itemToParse == null)
            {
                throw new ArgumentNullException("Attempted to parse null!");
            }

            string[] randoItemDetails = itemToParse.Split('-');
            if (randoItemDetails.Length == 3 && Enum.IsDefined(typeof(EItems), randoItemDetails[1]) && int.TryParse(randoItemDetails[2], out int quantity))
            {
                return new RandoItemRO(randoItemDetails[0], (EItems)Enum.Parse(typeof(EItems), randoItemDetails[1], true), quantity);
            }
            else
            {
                throw new ArgumentException($"Passed string '{itemToParse}' invalid for parsing. Expect three values in format 'name-item-quantity'.");
            }
        }
    }
}
