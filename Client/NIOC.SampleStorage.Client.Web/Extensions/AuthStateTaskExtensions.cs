using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Client.Web.Extensions
{
    public static class AuthStateTaskExtensions
    {
        public static async Task<List<string>> GetUserRoles(this Task<AuthenticationState> authStateTask)
        {
            var authState = await authStateTask;

            return authState.User.Claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).ToList();
        }
    }
}