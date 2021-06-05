using NIOC.SampleStorage.Shared.Core.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NIOC.SampleStorage.Shared.Core.Exceptions
{
    [Serializable]
    public class ValidationException : NIOCException
    {
        public ValidationException(params (string propName, string errorMsg)[] errors)
            : this(errors.Select(err => (err.propName, new[] { err.errorMsg })).ToArray())
        {

        }

        public ValidationException(params (string propName, string[] errorMsgs)[] errors) : this(new ModelErrorWrapper
        {
            Errors = new List<ATAModelError>(errors
                .Select(e => new ATAModelError
                {
                    Property = e.propName,
                    Messages = e.errorMsgs
                }))
        })
        {

        }

        public ValidationException(ModelErrorWrapper error)
            : base(message: "Validation Failed")
        {
            Error = error;
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ModelErrorWrapper? Error { get; }
    }
}