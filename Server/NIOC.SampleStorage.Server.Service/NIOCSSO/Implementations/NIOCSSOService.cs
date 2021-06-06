using NIOC.SampleStorage.Server.Service.NIOCSSO.Contracts;
using NIOC.SampleStorage.Server.Service.NIOCSSO.Dtos;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Service.NIOCSSO.Implementations
{
    public class NIOCSSOService : INIOCSSOService
    {
        private readonly HttpClient _client;

        public NIOCSSOService(SSOClient singleSignOnClient)
        {
            _client = singleSignOnClient.Client;
        }

        public async Task<DomainUsernameResponseDto> GetUserDomainNameAsync(DomainUsernameRequestDto domainUsernameRequest, CancellationToken cancellationToken)
        {
            var response =
                await _client.GetFromJsonAsync<DomainUsernameResponseDto>(
                    $"/api/someAddress/{domainUsernameRequest.Username}", cancellationToken);

            if (response is null)
                throw new HttpRequestException("خطا در اتصال به سامانه احراز هویت اکتشاف");

            return response;
        }

        public async Task<UserSSODataResponseDto> GetUserSSODataAsync(UserSSODataRequestDto userDataRequest, CancellationToken cancellationToken)
        {
            var response =
                await _client.GetFromJsonAsync<UserSSODataResponseDto>($"api/rest/{userDataRequest.UserDomainName}", cancellationToken);

            if (response is null)
                throw new HttpRequestException("خطا در اتصال به سامانه احراز هویت اکتشاف");

            return response;
        }
    }
}