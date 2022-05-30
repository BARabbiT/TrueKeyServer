using System;
using System.Runtime.Serialization;

namespace TrueKeyServer.Support
{
    [Serializable]
    internal class WarnException : Exception
    {
        public WarnException()
        {
        }

        public WarnException(string message) : base(message)
        {
        }

        public WarnException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WarnException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}