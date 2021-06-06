using Bit.Core.Exceptions;
using Bit.OData.ODataControllers;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Shared.App;
using NIOC.SampleStorage.Shared.Core.Dto.Identity;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace NIOC.SampleStorage.Server.Api.Controllers.Identity
{
    [AllowAnonymous]
    public class IdentityController : DtoController
    {
        public ServerAppSettings ServerAppSettings { get; set; } = default!; //Property Injection

        [Action]
        public async Task<LoginJwtDto> Login(LoginDto loginArg, CancellationToken cancellationToken)
        {
            var appClient = new HttpClient { BaseAddress = new Uri(ServerAppSettings.UrlOptions!.AppAddress!) };

            var reqContent = $"scope=openid+profile+user_info&grant_type=password&username={loginArg.UserDomainName}&password={loginArg.HeaderKey}&client_id={AppConstants.WebApp.ClientId}&client_secret=secret";

            var buffer = System.Text.Encoding.UTF8.GetBytes(reqContent);

            using var byteContent = new ByteArrayContent(buffer);

            using HttpResponseMessage response = await appClient.PostAsync("/core/connect/token", byteContent, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using HttpContent responseContent = response.Content;

                return await responseContent.ReadAsAsync<LoginJwtDto>(cancellationToken: cancellationToken);
            }

            throw new UnauthorizedException("نام کاربری و یا کلمه عبور اشتباه است");
        }
    }
}