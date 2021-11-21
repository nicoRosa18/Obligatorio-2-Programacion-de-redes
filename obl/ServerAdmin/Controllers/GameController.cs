using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerAdmin.AdminLogic;

namespace ServerAdmin.Controllers
{
    [ApiController]
    [Route("game")]
    public class GameController : ControllerBase
    {
        private readonly ILogic _logic;

        public GameController(ILogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<int>>> Get()
        {
            //int test = await _logic.TestMethodAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> NewGame()
        {
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ModifyGame()
        {
            return Ok();
        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveGame()
        {

            return Ok();
        }
    }
}
