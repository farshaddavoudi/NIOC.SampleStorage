using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Client.Web.Extensions
{
    public static class JsRuntimeExtensions
    {
        public static async Task AlertAsync(this IJSRuntime jsRuntime, string alertMessage)
        {
            await jsRuntime.InvokeVoidAsync("alert", alertMessage);
        }

        public static async Task SetCookieAsync(this IJSRuntime jsRuntime, string cookieName, string cookieValue, double expireMinutes = 525600)
        {
            await jsRuntime.InvokeVoidAsync("setCookie", cookieName, cookieValue, expireMinutes);
        }

        public static async Task<string> GetCookieAsync(this IJSRuntime jsRuntime, string cookieName)
        {
            return await jsRuntime.InvokeAsync<string>("getCookie", cookieName);
        }

        public static async Task<string> DeleteCookieAsync(this IJSRuntime jsRuntime, string cookieName)
        {
            return await jsRuntime.InvokeAsync<string>("deleteCookie", cookieName);
        }

        public static async Task OpenInNewTabAsync(this IJSRuntime jsRuntime, string url)
        {
            await jsRuntime.InvokeVoidAsync("open", url, "_blank");
        }

        public static async Task ChangeAddressBarUrlAsync(this IJSRuntime jsRuntime, string newUrl)
        {
            await jsRuntime.InvokeVoidAsync("changeAddressBarUrl", newUrl);
        }

        public static async Task AddCSSClassByElementIdAsync(this IJSRuntime jsRuntime, string elementId, string className)
        {
            await jsRuntime.InvokeVoidAsync("addClassToElementById", elementId, className);
        }

        public static async Task SetFocusAsync(this IJSRuntime jsRuntime, string elementId)
        {
            await jsRuntime.InvokeVoidAsync("setFocus", elementId);
        }

    }
}