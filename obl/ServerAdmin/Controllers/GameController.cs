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
        
        [HttpPost]
        public async Task<IActionResult> NewGame([FromBody] GameDTO game)
        {
            await _logic.AddGameAsync(game);
            return Ok(game);
        }

        [HttpPut("{title}")]
        public async Task<IActionResult> ModifyGame([FromRoute] string title, [FromBody] GameDTO game)
        {
            await _logic.ModifyGameAsync(title, game);
            return Ok(game);
        }

        [HttpDelete("{title}")]
        public async Task<IActionResult> RemoveGame([FromRoute] string title)
        {
            await _logic.RemoveGameAsync(title);
            return Ok($"Game {title} deleted successfully!");
        }
    }
}