using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerAdmin.AdminLogic;
using ServerAdmin.DTOs;

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

        [HttpGet("{title}")]
        public async Task<ActionResult<ICollection<int>>> Get([FromRoute] string title)
        {
            Task game = _logic.GetGame(title);
            return Ok();
        }

        [HttpPost]
        public IActionResult NewGame([FromBody] GameDTO game)
        {
            Task task = _logic.AddGameAsync(game);
            return Ok(task);
        }

        [HttpPut("{title}")]
        public async Task<IActionResult> ModifyGame([FromRoute] string title, [FromBody] GameDTO game)
        {
            await _logic.ModifyGameAsync(title, game);
            return Ok();
        }

        [HttpDelete("{title}")]
        public async Task<IActionResult> RemoveGame([FromRoute] string title)
        {
            await _logic.RemoveGameAsync(title);
            return Ok();
        }
    }
}