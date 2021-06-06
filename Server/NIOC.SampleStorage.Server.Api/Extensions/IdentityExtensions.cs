using NIOC.SampleStorage.Shared.Core.Extensions;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace NIOC.SampleStorage.Server.Api.Extensions
{
    public static class IdentityExtensions
    {
        public static string? FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.FindFirst(claimType)?.Value;
        }

        public static string? FindFirstValue(this IIdentity identity, string claimType)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue(claimType);
        }

        public static string? GetUserDomainName(this IIdentity identity)
        {
            return identity.FindFirstValue("sub")?.Trim();
        }

        public static T? GetUserDomainName<T>(this IIdentity identity) where T : IConvertible
        {
            var userDomainName = identity.GetUserDomainName();
            return userDomainName.IsNotNullOrEmpty()
                ? (T)Convert.ChangeType(userDomainName, typeof(T), CultureInfo.InvariantCulture)!
                : default(T);
        }

        public static string? GetUserName(this IIdentity identity)
        {
            return identity.FindFirstValue(ClaimTypes.Name);
        }

        public static Guid? GetSubjectId(this IIdentity identity)
        {
            var userDomainName = identity.GetUserDomainName();
            if (userDomainName == null)
                return null;
            return Guid.Parse(userDomainName);
        }
    }
}
