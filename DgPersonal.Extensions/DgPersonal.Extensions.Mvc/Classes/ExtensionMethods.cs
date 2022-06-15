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
        
        public static T GetViewDataValue<T>(this ViewDataDictionary viewData, string key)
        {
            var obj = viewData[key];
            if (obj == null)
                return default;
            
            return (T) obj;
        }
        
        public static bool IsHttpGet(this FilterContext context)
            => context.ActionDescriptor.EndpointMetadata.FirstOrDefault(x =>
                x.GetType() == typeof(HttpGetAttribute)) != null;
        
        public static bool IsDevelopment(this ViewDataDictionary viewData)
            => viewData.GetViewDataValue<bool>("IsDevelopment");
        
        public static bool IsProduction(this ViewDataDictionary viewData)
            => viewData.IsDevelopment() == false;
        
        public static bool IsAdministrator(this ViewDataDictionary viewData)
            => viewData.GetViewDataValue<bool>("IsAdministrator");
    }
}