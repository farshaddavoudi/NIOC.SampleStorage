using Bit.Core.Contracts;
using NIOC.SampleStorage.Server.Api.ActionFilters;

namespace NIOC.SampleStorage.Server.Api.Installers
{
    public static class AppModuleExtensions
    {
        public static IDependencyManager RegisterModelStateValidator(this IDependencyManager dependencyManager)
        {
            dependencyManager.RegisterGlobalWebApiActionFiltersUsing(webApiConfig =>
                webApiConfig.Filters.Add(new ModelStateValidatorAttribute()));

            return dependencyManager;
        }
    }
}