using NIOC.SampleStorage.Shared.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NIOC.SampleStorage.Shared.Core.Helpers
{
    public static class EnumMapping
    {
        /// <summary>
        /// Get List of Enum All Values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ToEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Get List of SelectListItem from Enum. Item(s) Text is set on Enum DisplayName property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueIsEnumString">The value of result SelectListItem list would be the string value of Enum</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectListItems<T>(bool valueIsEnumString = false) where T : Enum
        {
            return ToEnumValues<T>().Select(e => new SelectListItem(
                e.ToDisplayName(), !valueIsEnumString ? e.ToString("d") : e.ToString())).ToList();
        }

        /// <summary>
        /// Get Dictionary from Enum. Dictionary value is set on Enum DisplayName property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string?> ToDictionary<T>() where T : System.Enum
        {
            return ToEnumValues<T>().ToDictionary(p => Convert.ToInt32(p), q => q.ToDisplayName());
        }

    }
}