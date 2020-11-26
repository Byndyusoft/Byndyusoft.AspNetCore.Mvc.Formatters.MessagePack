using Microsoft.AspNetCore.Mvc;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Functional.Controllers
{
    [Controller]
    [Route("msgpack-formatter")]
    public class MessagePackFormatterController : ControllerBase
    {
        [HttpPost]
        [Route("echo")]
        [FormatFilter]
        public object Echo([FromBody] object model)
        {
            return model;
        }
    }
}