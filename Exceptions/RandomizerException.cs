using System;

namespace MessengerRando.Exceptions
{
    /// <summary>
    /// Empty rando exception class used to keep track of errors
    /// </summary>
    public class RandomizerException : Exception
    {
        public RandomizerException()
        {

        }

        public RandomizerException(string message) : base(message)
        {

        }

        public RandomizerException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
