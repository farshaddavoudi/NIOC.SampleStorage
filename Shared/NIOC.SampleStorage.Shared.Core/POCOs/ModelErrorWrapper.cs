using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NIOC.SampleStorage.Shared.Core.POCOs
{
    public class ModelErrorWrapper
    {
        public List<NIOCModelError> Errors { get; set; } = new();
    }

    public class NIOCModelError
    {
        public NIOCModelError()
        {
            Property = "*";
        }

        public string? Property { get; set; }

        public IEnumerable<string> Messages { get; set; } = Array.Empty<string>();

        [JsonIgnore]
        public IEnumerable<Exception> Exceptions { get; set; } = Array.Empty<Exception>();
    }
}