using NIOC.SampleStorage.Shared.Core.Dto;
using System.Collections.Generic;

namespace NIOC.SampleStorage.Server.Model.Contracts
{
    public interface IUserInfoProvider
    {
        bool IsAuthenticated();

        UserDto? CurrentUser();

        string UserDomainName();

        List<string> UserRoles();

        public string? IpAddress();
    }
}
