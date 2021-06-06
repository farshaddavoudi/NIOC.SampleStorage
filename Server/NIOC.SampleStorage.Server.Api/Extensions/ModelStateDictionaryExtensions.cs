using NIOC.SampleStorage.Shared.Core.POCOs;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace NIOC.SampleStorage.Server.Api.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static ModelErrorWrapper GetModelErrors(this ModelStateDictionary dictionary)
        {
            var modelErrorWrapper = new ModelErrorWrapper();

            foreach ((string key, ModelState value) in dictionary.Where(HasError))
            {
                modelErrorWrapper.Errors.Add(new NIOCModelError
                {
                    Property = key,
                    Messages = ExtractErrorMessages(value.Errors),
                    Exceptions = value.Errors
                        .Where(err => err.Exception != null)
                        .Select(err => err.Exception)
                });
            }

            return modelErrorWrapper;
        }

        private static bool HasError(KeyValuePair<string, ModelState> modelStatePair)
        {
            ModelErrorCollection errors = modelStatePair.Value.Errors;

            return errors != null && errors.Any();
        }

        private static IEnumerable<string> ExtractErrorMessages(ModelErrorCollection errors)
        {
            return errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "پارامترهای ورودی معتبر نیست" : error.ErrorMessage);
        }
    }
}