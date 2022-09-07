using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

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
