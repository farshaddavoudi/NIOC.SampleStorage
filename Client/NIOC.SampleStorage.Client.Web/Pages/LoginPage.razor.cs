using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NIOC.SampleStorage.Client.Web.Extensions;
using NIOC.SampleStorage.Shared.Core.Dto.Identity;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Client.Web.Pages
{
    [AllowAnonymous]
    public partial class LoginPage
    {
        // Props
        private LoginDto Login { get; set; } = new();

        // Injects
        [Inject] public IJSRuntime JsRuntime { get; set; }

        // Life-Cycles
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JsRuntime.SetFocusAsync("usernameInput");

            await base.OnAfterRenderAsync(firstRender);
        }

        // Methods
        private async Task LoginMe()
        {

        }
    }
}
