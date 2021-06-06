using Bit.Owin.Contracts;
using Bit.WebApi.ActionFilters;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace NIOC.SampleStorage.Server.Api.ActionFilters
{
    public class NIOCExceptionHandlerFilterAttribute : ExceptionHandlerFilterAttribute
    {
        protected override HttpResponseMessage CreateErrorResponseMessage(HttpActionExecutedContext actionExecutedContext, IExceptionToHttpErrorMapper exceptionToHttpErrorMapper, Exception exception)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(exceptionToHttpErrorMapper.GetMessage(actionExecutedContext.Exception), Encoding.UTF8, "application/json"),
                StatusCode = exceptionToHttpErrorMapper.GetStatusCode(exception)
            };
        }
    }
}