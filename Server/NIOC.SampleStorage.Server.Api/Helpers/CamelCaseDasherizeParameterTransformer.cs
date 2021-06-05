using Humanizer;
using Microsoft.AspNetCore.Routing;

namespace NIOC.SampleStorage.Server.Api.Helpers
{
    public class CamelCaseDasherizeParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value?.ToString().Kebaberize();
        }
    }
}