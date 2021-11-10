using System;
using Microsoft.AspNetCore.Mvc;
using ServerLogs.Container;

namespace ServerLogs.Controllers
{
    [Route("filterlogs")]
    [ApiController]
    public class FilterLogsController : ControllerBase
    {
        private readonly LogContainer _container;

        public FilterLogsController()
        {

        }

        [HttpGet]
        public IActionResult Get([FromBody] SearchParameters search)
        {
            return Ok();
        }
    }
}
