using Bit.Core.Contracts;
using Bit.Core.Exceptions;
using Bit.Owin.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Model.Entities.Identity;
using NIOC.SampleStorage.Server.Service.Identity;
using NIOC.SampleStorage.Shared.App;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Api.Middleware
{
    public class IdentityTokenMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly string[] _identityTokenValidSubUris =
        {
            "/api",
            "/odata"
        };

        public IdentityTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_identityTokenValidSubUris.Any(m =>
                context.Request.Path.HasValue &&
                context.Request.Path.Value!.StartsWith(m, StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                try
                {
                    var userInformationProvider = context.RequestServices.GetRequiredService<IUserInformationProvider>();

                    if (userInformationProvider.IsAuthenticated())
                    {
                        var identityTokenId = Convert.ToInt32(
                            userInformationProvider.GetBitJwtToken()
                                .CustomProps
                                .ExtendedSingle("Finding identity token id", p => p.Key == AppConstants.IdentityTokenId)
                                .Value!
                        );

                        var identityTokenService =
                            context.RequestServices.GetRequiredService<IdentityTokenService>();

                        IdentityTokenEntity identityToken =
                            (await identityTokenService
                                .GetIdentityTokenById(identityTokenId, context.RequestAborted))!;

                        if (identityToken == null)
                            throw new UnauthorizedException();

                        var dateTimeProvider = context.RequestServices.GetRequiredService<IDateTimeProvider>();

                        if (dateTimeProvider.GetCurrentUtcDateTime() > identityToken.ExpiresAt)
                            throw new UnauthorizedException();

                    }
                }
                catch (Exception exp) // workaround
                {
                    var scopeStatusManager = context.RequestServices.GetRequiredService<IScopeStatusManager>();
                    var logger = context.RequestServices.GetRequiredService<ILogger>();
                    if (scopeStatusManager.WasSucceeded())
                        scopeStatusManager.MarkAsFailed(exp.Message);
                    logger.AddLogData("Request-Execution-Exception", exp);
                    string statusCode = context.Response.StatusCode.ToString(CultureInfo.InvariantCulture);
                    bool responseStatusCodeIsErrorCodeBecauseOfSomeServerBasedReason = statusCode.StartsWith("5", StringComparison.InvariantCultureIgnoreCase);
                    bool responseStatusCodeIsErrorCodeBecauseOfSomeClientBasedReason = statusCode.StartsWith("4", StringComparison.InvariantCultureIgnoreCase);
                    if (responseStatusCodeIsErrorCodeBecauseOfSomeClientBasedReason == false && responseStatusCodeIsErrorCodeBecauseOfSomeServerBasedReason == false)
                    {
                        IExceptionToHttpErrorMapper exceptionToHttpErrorMapper = context.RequestServices.GetRequiredService<IExceptionToHttpErrorMapper>();
                        context.Response.StatusCode = Convert.ToInt32(exceptionToHttpErrorMapper.GetStatusCode(exp), CultureInfo.InvariantCulture);
                        context.Features.Get<IHttpResponseFeature>().ReasonPhrase = exceptionToHttpErrorMapper.GetReasonPhrase(exp);
                        await context.Response.WriteAsync(exceptionToHttpErrorMapper.GetMessage(exp), context.RequestAborted);
                    }
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}