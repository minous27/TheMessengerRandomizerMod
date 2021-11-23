using System;
using System.Runtime.Serialization;

namespace MessengerRando.Exceptions
{
    public class RandomizerNoMoreLocationsException : Exception
    {
        public RandomizerNoMoreLocationsException()
        {
        }

        public RandomizerNoMoreLocationsException(string message) : base(message)
        {
        }

        public RandomizerNoMoreLocationsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RandomizerNoMoreLocationsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
