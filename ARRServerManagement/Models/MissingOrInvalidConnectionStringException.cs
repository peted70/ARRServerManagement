using System;
using System.Runtime.Serialization;

namespace ARRServerManagement.Models
{
    [Serializable]
    internal class MissingOrInvalidConnectionStringException : Exception
    {
        public MissingOrInvalidConnectionStringException()
        {
        }

        public MissingOrInvalidConnectionStringException(string message) : base(message)
        {
        }

        public MissingOrInvalidConnectionStringException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingOrInvalidConnectionStringException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}