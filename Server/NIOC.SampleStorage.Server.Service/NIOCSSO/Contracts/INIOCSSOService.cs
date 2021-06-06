using NIOC.SampleStorage.Server.Service.NIOCSSO.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Service.NIOCSSO.Contracts
{
    public interface INIOCSSOService
    {
        Task<DomainUsernameResponseDto> GetUserDomainNameAsync(DomainUsernameRequestDto domainUsernameRequest,
            CancellationToken cancellationToken);

        Task<UserSSODataResponseDto> GetUserSSODataAsync(UserSSODataRequestDto userDataRequest,
            CancellationToken cancellationToken);
    }
}