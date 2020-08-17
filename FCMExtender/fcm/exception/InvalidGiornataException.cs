using System;
using System.Runtime.Serialization;

namespace fcm.exception
{
    [Serializable]
    internal class InvalidGiornataException : Exception
    {
        public InvalidGiornataException()
        {
        }

        public InvalidGiornataException(string message) : base(message)
        {
        }

        public InvalidGiornataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidGiornataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}