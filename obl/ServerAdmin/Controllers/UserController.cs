using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerAdmin.AdminLogic;

namespace ServerAdmin.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly ILogic _logic;

        public UserController(ILogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public async Task<ActionResult<int>> Get()
        {
            int test = await _logic.TestMethodAsync();
            return Ok(test);
        }
    }
}
