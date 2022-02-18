using System.Linq;
using DgPersonal.Extensions.Mvc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DgPersonal.Extensions.Mvc.Classes
{
    public static class ExtensionMethods
    {
        public static string WithoutController(this string controllerName) 
            => controllerName?.Replace("Controller", "") ?? "";
        
        public static IActionResult ContentAsJson<TController>(this TController controller, object content)
            where TController : Controller, IJsonReturn
            => controller.ContentAsJson(controller, content);
        
        public static bool GetBoolViewDataValue(this ViewDataDictionary viewData, string key)
        {
            var entry = viewData[key];
            if (entry == null)
                return false;
            
            return (bool) entry;
        }
        
        public static bool IsHttpGet(this FilterContext context)
            => context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x =>
                x.GetType() == typeof(HttpGetAttribute)) != null;
    }
}