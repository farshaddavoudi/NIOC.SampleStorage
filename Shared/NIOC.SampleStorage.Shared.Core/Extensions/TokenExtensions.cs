using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NIOC.SampleStorage.Shared.Core.Dto.Identity;
using System;
using System.Text;

namespace NIOC.SampleStorage.Shared.Core.Extensions
{
    public static class TokenExtensions
    {
        public static PrimarySidDto? GetJwtTokenProps(this string jwtToken)
        {
            string[] token = jwtToken.Split(".");

            var decodedToken = ToBase64String(token[1]);

            JToken jToken = JToken.Parse(decodedToken);

            JObject o = JObject.Parse(jToken["primary_sid"]?.ToObject<dynamic>());

            var primarySidDtoJson = o.ToString(Formatting.None);

            return primarySidDtoJson.DeserializeToModel<PrimarySidDto>();
        }

        public static string ToBase64String(this string value)
        {
            string stringValue = value.Replace('-', '+').Replace('_', '/');

            while (stringValue.Length % 4 != 0)
            {
                stringValue += '=';
            }

            var base64String = Convert.FromBase64String(stringValue);

            return Encoding.UTF8.GetString(base64String);
        }
    }
}
