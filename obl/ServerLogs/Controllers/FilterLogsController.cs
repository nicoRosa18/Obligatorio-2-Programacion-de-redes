using System;
using System.Collections.Generic;
using CommonLogs;
using Microsoft.AspNetCore.Mvc;
using ServerLogs.Container;

namespace ServerLogs.Controllers
{
    [Route("filterlogs")]
    [ApiController]
    public class FilterLogsController : ControllerBase
    {
        private readonly ILogContainer _logContainer;

        public FilterLogsController(ILogContainer logContainer)
        {
            _logContainer = logContainer;
        }

        [HttpGet]
        public IActionResult Get([FromBody] SearchParameters search)
        {
            ICollection<Log> logs = _logContainer.ShowLogs();
            return Ok(logs);
        }
    }
}
