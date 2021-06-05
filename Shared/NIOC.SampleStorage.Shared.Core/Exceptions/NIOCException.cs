using Bit.Core.Exceptions.Contracts;
using System;
using System.Runtime.Serialization;

namespace NIOC.SampleStorage.Shared.Core.Exceptions
{
    public abstract class NIOCException : ApplicationException, IKnownException
    {
        protected NIOCException(string message)
            : base(message)
        {
        }

        protected NIOCException(Exception exception)
            : base(exception.Message, exception)
        {
        }

        protected NIOCException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NIOCException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}