using Bit.Core.Contracts;
using Microsoft.Owin;
using NIOC.SampleStorage.Server.Api.Extensions;
using NIOC.SampleStorage.Shared.Core.Exceptions;
using NIOC.SampleStorage.Shared.Core.POCOs;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NIOC.SampleStorage.Server.Api.ActionFilters
{
    public class ModelStateValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState is null)
                throw new ArgumentNullException(nameof(actionContext.ModelState));

            if (actionContext.ModelState.IsValid | actionContext.Request.Method == HttpMethod.Get /*workaround*/)
                return;

            IDependencyResolver dependencyResolver = actionContext.Request.GetOwinContext().GetDependencyResolver();
            var logger = dependencyResolver.Resolve<ILogger>();

            ModelErrorWrapper modelErrorWrapper = actionContext.ModelState.GetModelErrors();
            logger.AddLogData("ModelError", modelErrorWrapper);

            throw new ValidationException(modelErrorWrapper);
        }
    }
}