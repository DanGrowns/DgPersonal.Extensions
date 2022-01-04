using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DgPersonal.Extensions.Mvc.Interfaces
{
    public interface IViewRenderer
    {
        Task<string> RenderViewToStringAsync(Controller controller, string viewNamePath, object model = null);
    }
}