using Bit.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;

namespace NIOC.SampleStorage.Server.Api.ActionFilters
{
    public class NIOCAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public new string[] Roles { get; set; } = Array.Empty<string>();

        // AspNetCore Controllers
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool skipAuthorization = context.Filters.OfType<IAllowAnonymousFilter>().Any() ||
                                     context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any() ||
                                     context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

            if (skipAuthorization) return;

            var userInformationProvider = context.HttpContext.RequestServices.GetRequiredService<IUserInformationProvider>();
            var contentFormatter = context.HttpContext.RequestServices.GetRequiredService<IContentFormatter>();
            if (!userInformationProvider.IsAuthenticated())
            {
                context.Result = new ForbidResult();
                return;
            }

            var userRoles = contentFormatter.Deserialize<List<string>>(userInformationProvider.GetBitJwtToken().CustomProps["roles"] ?? string.Empty);

            // If user do not have any roles, so it mean he/she do not access to the app.
            if (userRoles.Count == 0)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!Roles.Any())
                return;

            if (!userRoles.Any(ur => Roles.Contains(ur)))
                context.Result = new ForbidResult();
        }

        // Bit Controllers
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException(nameof(actionContext));

            var resolver = actionContext.Request.GetOwinContext()
                .GetDependencyResolver();

            var userInformationProvider = resolver
                .Resolve<IUserInformationProvider>();

            var contentFormatter = resolver.Resolve<IContentFormatter>();

            if (!base.IsAuthorized(actionContext))
                return false;

            var userRoles = contentFormatter.Deserialize<List<string>>(userInformationProvider.GetBitJwtToken().CustomProps["roles"] ?? string.Empty);

            // If user do not have any roles, so it mean he/she do not access to the app.
            if (userRoles.Count == 0)
                return false;

            if (!Roles.Any())
                return true;

            return userRoles.Any(ur => Roles.Contains(ur));
        }
    }
}