using System;
using System.Runtime.Serialization;

namespace Fantasista.IronDotNet.Exceptions
{
    [Serializable]
    internal class IronMissingPasswordException : Exception
    {
        public IronMissingPasswordException()
        {
        }

        public IronMissingPasswordException(string message) : base(message)
        {
        }

        public IronMissingPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IronMissingPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}