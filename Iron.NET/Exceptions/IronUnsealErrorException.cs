using System;
using System.Runtime.Serialization;

namespace Fantasista.IronDotNet
{
    [Serializable]
    internal class IronUnsealErrorException : Exception
    {
        public IronUnsealErrorException()
        {
        }

        public IronUnsealErrorException(string message) : base(message)
        {
        }

        public IronUnsealErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IronUnsealErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}