using System;
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
        public async Task<ActionResult<ICollection<tipojuego>>> Get()
        {
            //int test = await _logic.TestMethodAsync();
            return Ok(test);
        }

        [HttpPost]
        public async Task<> NewGame()
        {

        }

        [HttpPut]
        public async Task<> ModifyGame()
        {

        }
        
        [HttpDelete]
        public async Task<IActionResult> RemoveGame()
        {

            return Ok();
        }

        

    }
}
