using Mod.Courier;
using System;
using System.Runtime.Serialization;

namespace MessengerRando.Exceptions
{
    public class RandomizerException : Exception
    {
        public RandomizerException() : base() => LogToConsole(null);

        public RandomizerException(string message) : base(message)
            => LogToConsole(message);

        public RandomizerException(string message, Exception innerException) : base(message, innerException)
            => LogToConsole($"{message} | Inner: {innerException.Message}");

        protected RandomizerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Usually no need to log during deserialization
        }

        private void LogToConsole(string detail)
        {
            CourierLogger.Log("Randomizer Exception", $"{detail}");
        }
    }
}
