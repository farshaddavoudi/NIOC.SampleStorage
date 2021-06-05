using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using NIOC.SampleStorage.Shared.App;
using System.Collections.Generic;

namespace NIOC.SampleStorage.Server.Api.Identity
{
    public class IdentityClientsProvider : OAuthClientsProvider
    {
        public override IEnumerable<IdentityServer3.Core.Models.Client> GetClients()
        {
            return new[]
            {
                GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
                {
                    ClientName = AppConstants.WebApp.ClientName,
                    ClientId = AppConstants.WebApp.ClientId,
                    Secret = "secret",
                    TokensLifetime = AppConstants.JwtTokenLifetime,
                    Enabled = true
                })
            };
        }
    }
}
