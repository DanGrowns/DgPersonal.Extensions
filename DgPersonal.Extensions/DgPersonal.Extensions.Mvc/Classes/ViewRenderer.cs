using System;
using System.IO;
using System.Threading.Tasks;
using DgPersonal.Extensions.Mvc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DgPersonal.Extensions.Mvc.Classes
{
    public class ViewRenderer : IViewRenderer
    {
        private static IView FindView(ControllerBase controller, string viewNamePath)
        {
            IViewEngine viewEngine =
                controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            if (viewEngine == null)
                throw new ArgumentException("ViewEngine was null, cannot continue to render a view from a controller.");

            var viewResult = viewNamePath.EndsWith(".cshtml") 
                ? viewEngine.GetView(viewNamePath, viewNamePath, false) 
                : viewEngine.FindView(controller.ControllerContext, viewNamePath, false);

            if (viewResult.Success) 
                return viewResult.View;
            
            var endPointDisplay = controller.HttpContext.GetEndpoint()?.DisplayName ?? "";

            if (endPointDisplay.Contains(".Areas."))
            {
                //search in Areas
                var areaName = endPointDisplay.Substring(endPointDisplay.IndexOf(".Areas.", StringComparison.Ordinal) + ".Areas.".Length);
                areaName = areaName.Substring(0, areaName.IndexOf(".Controllers.", StringComparison.Ordinal));

                viewNamePath =
                    $"~/Areas/{areaName}/views/{controller.HttpContext.Request.RouteValues["controller"]}/{controller.HttpContext.Request.RouteValues["action"]}.cshtml";

                viewResult = viewEngine.GetView(viewNamePath, viewNamePath, false);
            }

            if (!viewResult.Success)
                throw new Exception($"A view with the name '{viewNamePath}' could not be found");

            return viewResult.View;
        }
        
        /// <summary>
        /// Render a partial view to string.
        /// </summary>
        public async Task<string> RenderViewToStringAsync(Controller controller, string viewNamePath, object model = null)
        {
            if (string.IsNullOrEmpty(viewNamePath))
                viewNamePath = controller.ControllerContext.ActionDescriptor.ActionName;

            controller.ViewData.Model = model;

            await using var writer = new StringWriter();

            try
            {
                var view = FindView(controller, viewNamePath);

                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    view,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
            catch (Exception exc)
            {
                return $"Failed - {exc.Message}";
            }
        }
    }
}