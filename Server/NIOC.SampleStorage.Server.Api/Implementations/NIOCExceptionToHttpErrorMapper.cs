using Bit.Owin.Implementations;
using Bit.Owin.Metadata;
using NIOC.SampleStorage.Server.Api.Extensions;
using NIOC.SampleStorage.Shared.Core.Exceptions;
using System;
using System.Net;

namespace NIOC.SampleStorage.Server.Api.Implementations
{
    public class NIOCExceptionToHttpErrorMapper : DefaultExceptionToHttpErrorMapper
    {
        public override HttpStatusCode GetStatusCode(Exception exp)
        {
            if (exp is ValidationException)
                return HttpStatusCode.BadRequest;

            //if (exp is InvalidTokenException)
            //    return HttpStatusCode.BadRequest;

            return base.GetStatusCode(exp);
        }

        public override string GetMessage(Exception exp)
        {
            exp = UnWrapException(exp);

            string? messageToShow = BitMetadataBuilder.UnknownError;

            if (IsKnownError(exp))
            {
                messageToShow = exp.GetModelErrorWrapper();
            }

            return messageToShow ?? "";
        }
    }
}