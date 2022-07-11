using System;
using System.Runtime.Serialization;

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

        public RandomizerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RandomizerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
