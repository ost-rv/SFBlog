using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace SFBlog.Extensions
{
    public static class ModelStateDictionaryExtension
    {
        public static string GetAllError(this ModelStateDictionary modelState)
        {
            List<string> errorList = new List<string>();

            errorList.AddRange(modelState.Root.Errors
                .Select(e => e.ErrorMessage)
                .ToList());


            errorList.AddRange(modelState.Values.SelectMany(m => m.Errors)
                .Select(e => e.ErrorMessage)
                .ToList());

            return string.Join(";\\r\\n", errorList);
        }
    }
}
