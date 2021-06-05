using Bit.Core.Implementations;
using Bit.Http.Contracts;
using Bit.Model.Contracts;
using Bit.Model.Implementations;
using Bit.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NIOC.SampleStorage.Shared.App;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable AdminPanelNamespace

namespace Bit.Model.Contracts
{
    public interface IDto
    {

    }
}

namespace Bit.Http.Contracts
{
    public class ODataContext
    {
        public string? Query { get; set; }

        public long? TotalCount { get; set; }
    }

    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public virtual T Value { get; set; } = default!;

        [JsonPropertyName("@odata.context")]
        public virtual string? Context { get; set; }

        /// <summary>
        /// It can be requested by $count=true in query string of your request.
        /// </summary>
        [JsonPropertyName("@odata.count")]
        public virtual long? TotalCount { get; set; }
    }
}

namespace Bit.Http.Contracts
{
    public class Token
    {
        public string access_token { get; set; } = default!;

        public string token_type { get; set; } = default!;

        public long expires_in { get; set; }

        public DateTimeOffset? login_date { get; set; }
    }

    public interface ISecurityService
    {
        Task<Token> LoginWithCredentials(string userName, string password, string client_id, string client_secret, string[]? scopes = null, IDictionary<string, string?>? acr_values = null, CancellationToken cancellationToken = default);
    }

    public interface ITokenProvider
    {
        Task<Token?> GetTokenAsync();

        Task SetTokenAsync(Token? token);
    }
}

namespace Bit.Http.Implementations
{
    public class DefaultSecurityService : ISecurityService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;

        public DefaultSecurityService(HttpClient httpClient, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
        }

        public virtual async Task<Token> LoginWithCredentials(string userName, string password, string client_id, string client_secret, string[]? scopes = null, IDictionary<string, string?>? acr_values = null, CancellationToken cancellationToken = default)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (client_id == null)
            {
                throw new ArgumentNullException(nameof(client_id));
            }
            if (client_secret == null)
            {
                throw new ArgumentNullException(nameof(client_secret));
            }

            scopes = scopes ?? new[] { "openid", "profile", "user_info" };

            string loginData = $"scope={string.Join("+", scopes)}&grant_type=password&username={userName}&password={password}&client_id={client_id}&client_secret={client_secret}";

            if (acr_values != null)
            {
                loginData += $"&acr_values={string.Join(" ", acr_values.Select(p => $"{p.Key}:{p.Value}"))}";
            }

            loginData = Uri.EscapeUriString(loginData);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "core/connect/token");

            request.Content = new StringContent(loginData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            request.Content.Headers.ContentLength = loginData.Length;

            using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            await using Stream responseContent = await response.EnsureSuccessStatusCode().Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

            Token token = await DefaultJsonContentFormatter.Current.DeserializeAsync<Token>(responseContent, cancellationToken).ConfigureAwait(false);

            token.login_date = DateTimeOffset.UtcNow;

            await _tokenProvider.SetTokenAsync(token).ConfigureAwait(false);

            return token;
        }
    }

    public class DefaultTokenProvider : ITokenProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public DefaultTokenProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<Token?> GetTokenAsync()
        {
            var access_token = await _jsRuntime.InvokeAsync<string>("getCookie", "access_token").ConfigureAwait(false);

            if (access_token == null)
                return null;

            return new Token { access_token = access_token, token_type = "Bearer" };
        }

        public async Task SetTokenAsync(Token? token)
        {
            if (token != null)
            {
                await _jsRuntime.InvokeVoidAsync("setCookie", "access_token", token.access_token, AppConstants.JwtTokenLifetime.TotalMinutes).ConfigureAwait(false);
                await _jsRuntime.InvokeVoidAsync("setCookie", "token_type", "Bearer", AppConstants.JwtTokenLifetime.TotalMinutes).ConfigureAwait(false);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("removeCookie", "access_token").ConfigureAwait(false);
                await _jsRuntime.InvokeVoidAsync("removeCookie", "token_type").ConfigureAwait(false);
            }
        }
    }

    public class ODataHttpClient<TDto>
        where TDto : IDto
    {
        public virtual string ODataRoute { get; }
        public virtual string ControllerName { get; }
        public virtual HttpClient HttpClient { get; }

        public ODataHttpClient(HttpClient httpClient, string controllerName, string odataRoute)
        {
            ODataRoute = odataRoute;
            ControllerName = controllerName;
            HttpClient = httpClient;
        }

        protected virtual async Task<TDto> SendAsync(object[] keys, object dto, HttpMethod method, ODataContext? oDataContext, CancellationToken cancellationToken)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            string qs = oDataContext?.Query is not null ? $"?{oDataContext.Query}" : string.Empty;

            using StringContent content = new StringContent(DefaultJsonContentFormatter.Current.Serialize(dto), Encoding.UTF8, DefaultJsonContentFormatter.Current.ContentType);

            using HttpRequestMessage request = new HttpRequestMessage(method, $"odata/{ODataRoute}/{ControllerName}({string.Join(",", keys)}){qs}");

            request.Content = content;

            using Stream responseStream = await (await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                .EnsureSuccessStatusCode()
                .Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            return await DeserializeAsync<TDto>(responseStream, null, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TDto> Create(TDto dto, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            return await SendAsync(Array.Empty<object>(), dto, HttpMethod.Post, oDataContext, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TDto> Update(TDto dto, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            return await SendAsync(DtoMetadataWorkspace.Current.GetKeys(dto), dto, HttpMethod.Put, oDataContext, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TDto> PartialUpdate(object[] keys, object dto, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            return await SendAsync(keys, dto, new HttpMethod("Patch"), oDataContext, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<TDto> PartialUpdate(TDto dto, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            return await SendAsync(DtoMetadataWorkspace.Current.GetKeys(dto), dto, new HttpMethod("Patch"), oDataContext, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task Delete(object[] keys, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));

            string qs = oDataContext?.Query is not null ? $"?{oDataContext.Query}" : string.Empty;

            (await HttpClient.DeleteAsync($"odata/{ODataRoute}/{ControllerName}({string.Join(",", keys)}){qs}", cancellationToken).ConfigureAwait(false))
                .EnsureSuccessStatusCode();
        }

        public virtual async Task<TDto> Get(object[] keys, ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));

            string qs = oDataContext?.Query is not null ? $"?{oDataContext.Query}" : string.Empty;

            using Stream responseStream = await (await HttpClient.GetAsync($"odata/{ODataRoute}/{ControllerName}({string.Join(",", keys)}){qs}", cancellationToken).ConfigureAwait(false))
                .EnsureSuccessStatusCode()
                .Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            return await DeserializeAsync<TDto>(responseStream, null, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task<List<TDto>> Get(ODataContext? oDataContext = default, CancellationToken cancellationToken = default)
        {
            string qs = oDataContext?.Query is not null ? $"?{oDataContext.Query}" : string.Empty;

            using Stream responseStream = await (await HttpClient.GetAsync($"odata/{ODataRoute}/{ControllerName}(){qs}", cancellationToken).ConfigureAwait(false))
                .EnsureSuccessStatusCode()
                .Content
                .ReadAsStreamAsync().ConfigureAwait(false);

            List<TDto> odataResponse = await DeserializeAsync<List<TDto>>(responseStream, oDataContext, cancellationToken).ConfigureAwait(false);

            return odataResponse;
        }

        public virtual async Task<T> DeserializeAsync<T>(Stream responseStream, ODataContext? context, CancellationToken cancellationToken)
        {
            using StreamReader streamReader = new StreamReader(responseStream);
            JsonElement json = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream, new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            }).ConfigureAwait(false);

            if (!json.TryGetProperty("value", out JsonElement _))
                return await json.ToObjectAsync<T>().ConfigureAwait(false);

            ODataResponse<T> oDataResponse = await json.ToObjectAsync<ODataResponse<T>>().ConfigureAwait(false);

            if (context != null)
                context.TotalCount = oDataResponse.TotalCount;

            return oDataResponse.Value;
        }
    }
}

namespace System.Text.Json
{
    public static partial class JsonExtensions
    {
        public static async Task<T> ToObjectAsync<T>(this JsonElement element, JsonSerializerOptions? options = null)
        {
            ArrayBufferWriter<byte>? bufferWriter = new ArrayBufferWriter<byte>();

            await using (Utf8JsonWriter writer = new Utf8JsonWriter(bufferWriter))
                element.WriteTo(writer);

            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options)!;
        }

        public static async Task<T> ToObjectAsync<T>(this JsonDocument document, JsonSerializerOptions? options = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return await document.RootElement.ToObjectAsync<T>(options).ConfigureAwait(false);
        }
    }
}

namespace Bit.Core.Implementations
{
    public class DefaultJsonContentFormatter
    {
        public static DefaultJsonContentFormatter Current { get; } = new DefaultJsonContentFormatter { };

        public async Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public virtual string Serialize<T>([AllowNull] T obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            });
        }

        public virtual string ContentType => "application/json";
    }
}

namespace Bit.Model.Implementations
{
    public class DtoMetadataWorkspace
    {
        public static DtoMetadataWorkspace Current { get; } = new DtoMetadataWorkspace { };

        public virtual PropertyInfo[] GetKeyColums(TypeInfo typeInfo)
        {
            bool IsKeyByConvention(PropertyInfo prop)
            {
                return string.Compare(prop.Name, "Id", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(prop.Name, (typeInfo.Name + "Id"), StringComparison.OrdinalIgnoreCase) == 0;
            }

            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo));

            PropertyInfo[] props = typeInfo.GetProperties();

            PropertyInfo[] keys = props
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null)
                .ToArray();

            if (keys.Any())
                return keys;
            else
                return props.Where(IsKeyByConvention).ToArray();
        }

        public virtual object[] GetKeys(IDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            TypeInfo dtoType = dto.GetType().GetTypeInfo();

            PropertyInfo[] props = GetKeyColums(dtoType);

            return props.Select(p => p.GetValue(dto)).ToArray()!;
        }
    }
}

namespace Bit_.View
{
    public class BitComponentBase : ComponentBase
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public ISecurityService SecurityService { get; set; }

        [Inject]
        public ITokenProvider TokenProvider { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public BitExceptionHandler ExceptionHandler => BitExceptionHandler.Current;

        protected sealed override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                await OnInitializedAsync(default);
            }
            catch (Exception exp)
            {
                ExceptionHandler.OnExceptionReceived(exp);
            }
        }

        protected virtual Task OnInitializedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual T Evaluate<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception exp)
            {
                ExceptionHandler.OnExceptionReceived(exp);
                return default;
            }
        }

        public virtual Func<Task> Invoke(Func<Task> func)
        {
            return async () =>
            {
                try
                {
                    await func();
                }
                catch (Exception exp)
                {
                    ExceptionHandler.OnExceptionReceived(exp);
                }
            };
        }


        public virtual Action Invoke(Action action)
        {
            return () =>
            {
                try
                {
                    action();
                    Invoke(StateHasChanged); // workaround
                }
                catch (Exception exp)
                {
                    ExceptionHandler.OnExceptionReceived(exp);
                }
            };
        }

        public virtual Func<EventArgs, Task> Invoke(Func<EventArgs, Task> func)
        {
            return async (e) =>
            {
                try
                {
                    await func(e);
                }
                catch (Exception exp)
                {
                    ExceptionHandler.OnExceptionReceived(exp);
                }
            };
        }
    }

    [Authorize]
    public class BitPageBase : BitComponentBase
    {

    }
}

namespace Bit.ViewModel
{
    public class BitExceptionHandler
    {
        public static BitExceptionHandler Current { get; } = new BitExceptionHandler { };

        public void OnExceptionReceived(Exception exp)
        {
#if DEBUG
            Console.WriteLine(exp.ToString());
#endif
        }
    }
}

namespace Bit.Core.Exceptions
{
    [Serializable]
    public class LoginFailureException : ApplicationException
    {
        public LoginFailureException()
        {
        }

        public LoginFailureException(string message)
            : base(message)
        {
        }

        public LoginFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LoginFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}