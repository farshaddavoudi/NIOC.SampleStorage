using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIOC.SampleStorage.Shared.App
{
    public class DefaultRole
    {
        public string? Name { get; set; }

        public IEnumerable<string> Claims { get; set; } = Array.Empty<string>();
    }

    public class DefaultRoles
    {
        public static DefaultRole Administrator => new()
        {
            Name = nameof(Administrator),
            Claims = Claims.GetAllClaimNames()
        };
    }

    public static class Claims
    {
        public static readonly string ManageUsersPermissions = nameof(ManageUsersPermissions);
        public static readonly string SeeReports = nameof(SeeReports);

        private static string[]? _claimNames;

        public static IEnumerable<string> GetAllClaimNames()
        {
            if (_claimNames is null)
            {
                _claimNames = typeof(Claims)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(field => (string?)field.GetValue(null))
                    .ToArray();
            }

            return _claimNames;
        }
    }
}