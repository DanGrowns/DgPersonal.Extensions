using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DgPersonal.Extensions.Mvc.Interfaces
{
    public interface IJsonReturn
    {
        IActionResult ContentAsJson(Controller controller, object content)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            var json = JsonConvert.SerializeObject(content, options);
            return controller.Content(json, "application/json");
        }
    }
}