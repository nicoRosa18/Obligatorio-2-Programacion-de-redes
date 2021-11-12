using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ActionResult<ICollection<Log>>> Get([FromBody] SearchParameters search)
        {
            ICollection<Log> taskLogs = await _logContainer.FilterLogsAsync(search.UserName, search.GameName, search.Date);
            return Ok(taskLogs);
        }
    }
}
