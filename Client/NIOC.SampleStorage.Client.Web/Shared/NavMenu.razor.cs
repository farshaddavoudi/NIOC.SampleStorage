using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Client.Web.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateTask;

            if (authState.User.Identity is null)
                return;

            if (authState.User.Identity.IsAuthenticated)
            {
                // Do something
            }

            await base.OnInitializedAsync();
        }
    }
}
