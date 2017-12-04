using System;
using System.Runtime.Serialization;

namespace Fantasista.IronDotNet.Exceptions
{
    [Serializable]
    internal class IronConfigurationErrorException : Exception
    {
        public IronConfigurationErrorException()
        {
        }

        public IronConfigurationErrorException(string message) : base(message)
        {
        }

        public IronConfigurationErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IronConfigurationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}