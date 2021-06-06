using Bit.Http.Contracts;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Extensions;
using NIOC.SampleStorage.Shared.App;
using NIOC.SampleStorage.Shared.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Client.Web.Implementations
{
    public class AppAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _hostClient;
        private readonly NavigationManager _navigationManager;
        private readonly ClientAppSettings _clientAppSettings;
        private readonly IWebAssemblyHostEnvironment _hostEnvironment;
        private readonly ILocalStorageService _localStorageService;
        private readonly ISecurityService _securityService;

        internal string RefreshBitToken = "";
        private readonly string _authTokenKey = AppConstants.SSOTokenKey;

        #region Constructor Injection

        public AppAuthenticationStateProvider(ITokenProvider tokenProvider,
            IJSRuntime jsRuntime,
            HttpClient hostClient,
            NavigationManager navigationManager,
            ClientAppSettings clientAppSettings,
            IWebAssemblyHostEnvironment hostEnvironment,
            ILocalStorageService localStorageService,
            ISecurityService securityService)
        {
            _tokenProvider = tokenProvider;
            _jsRuntime = jsRuntime;
            _hostClient = hostClient;
            _navigationManager = navigationManager;
            _clientAppSettings = clientAppSettings;
            _hostEnvironment = hostEnvironment;
            _localStorageService = localStorageService;
            _securityService = securityService;
        }

        #endregion

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Token token;

            bool refreshBitToken;
            try
            {
                refreshBitToken = await _localStorageService.GetItemAsync<bool>(nameof(RefreshBitToken));
            }
            catch
            {
                await _localStorageService.SetItemAsync(nameof(RefreshBitToken), false);
                refreshBitToken = false;
            }

            if (refreshBitToken)
            {
                // 1. Get SSOToken from Cookie
                var ssoToken = await _jsRuntime.GetCookieAsync(_authTokenKey);

                if (string.IsNullOrWhiteSpace(ssoToken))
                {
                    await NavigateToSSOLoginPage();
                    ssoToken = await _jsRuntime.GetCookieAsync(_authTokenKey);
                }

                // 2. Get/Set JwtToken by calling SecurityService.LoginWithCredentials method
                token = await _securityService.LoginWithCredentials(ssoToken!, AppMetadata.AppEnglishFullName, AppConstants.WebApp.ClientId, AppConstants.WebApp.Secret);

                // 3. Change back RefreshBitToken localstorage to false
                await _localStorageService.SetItemAsync(nameof(RefreshBitToken), false);
            }
            else
            {
                // 1. Get JwtToken by calling _tokenProvider.GetTokenAsync()
                token = await _tokenProvider.GetTokenAsync();

                if (token == null || string.IsNullOrWhiteSpace(token.access_token))
                {
                    // Get SSOToken from Cookie
                    var ssoToken = await _jsRuntime.GetCookieAsync(_authTokenKey);

                    if (ssoToken == null || string.IsNullOrWhiteSpace(ssoToken))
                    {
                        await NavigateToSSOLoginPage();

                        if (_hostEnvironment.IsDevelopment())
                            ssoToken = await _jsRuntime.GetCookieAsync(_authTokenKey);
                    }

                    // Call SSO client to confirm token
                    //UserByTokenData? AdminPanelSSOToken = default;
                    try
                    {
                        //AdminPanelSSOToken =
                        //    await _ssoClientService.GetUserByTokenAsync(ssoToken!, default);
                    }
                    catch
                    {
                        await NavigateToSSOLoginPage();
                    }

                    // No valid response or token has been expired
                    //if (AdminPanelSSOToken == null)
                    //    await NavigateToSSOLoginPage();

                    // SSOToken is valid at this point
                    token = await _securityService.LoginWithCredentials(ssoToken!, AppMetadata.AppEnglishFullName, AppConstants.WebApp.ClientId, AppConstants.WebApp.Secret);
                }

                // Set RefreshBitToken localstorage to false (to be ready for change)
                await _localStorageService.SetItemAsync(nameof(RefreshBitToken), false);
            }

            // Decode the JwtToken and export Claims / Roles from it 
            var primarySidDto = token.access_token.GetJwtTokenProps();
            var customProps = primarySidDto?.CustomProps;

            Console.WriteLine($"MyInfo={primarySidDto.SerializeToJson()}");

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{customProps!.FirstName} {customProps.LastName}"),
                new Claim(ClaimTypes.NameIdentifier, primarySidDto?.UserDomainName!),
                new Claim(AppConstants.Claims.PersonnelCode, customProps.PersonnelCode!),
                new Claim(AppConstants.Claims.UnitName, customProps.UnitName ?? "نامشخص"),
                new Claim(AppConstants.Claims.JobTitle, customProps.JobTitle ?? "نامشخص")
            };

            var roles = customProps.Roles?.DeserializeToModel<List<string>>();

            if (roles != null)
                foreach (var role in roles)
                {
                    if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                        claims.Add(new Claim(ClaimTypes.Role, role));
                }


            // Creates ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(claims, "Bearer");

            // Creates ClaimsPrinciple
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            return new AuthenticationState(claimsPrinciple);
        }

        private async Task NavigateToSSOLoginPage()
        {
            if (_hostEnvironment.IsDevelopment())
            {
                var securitySSOToken = "";
                //    await _ssoClientService.GetUserTokenByPersonnelCodeAsync(980923, CancellationToken.None);

                // Set auth cookie
                await _jsRuntime.SetCookieAsync(_authTokenKey, securitySSOToken, AppConstants.JwtTokenLifetime.TotalMinutes);
            }
            else
            {
                // Remove SSOToken
                await _jsRuntime.DeleteCookieAsync(_authTokenKey);

                // Navigate to SSOLogin
                //_navigationManager.NavigateTo(_ssoClientService.GetSSOLoginPageUrl(_clientAppSettings.UrlOptions!.AppAddress!));
            }
        }

    }
}
