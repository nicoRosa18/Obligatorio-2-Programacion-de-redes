using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServerAdmin.AdminLogic;
using ServerAdmin.DTOs;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<int>> Get([FromRoute] string id)
        {
            await _logic.GetUser(id);
            return Ok();
        }

        [HttpPost("{newUserName}")]
        public async Task<IActionResult> NewUser([FromRoute] string newUserName)
        {
            await _logic.AddUserAsync(newUserName);
            return Ok();
        }

        [HttpPut("{oldUserName}")]
        public async Task<IActionResult> ModifyUser([FromRoute] string oldUserName, [FromBody] string userName)
        {
            await _logic.ModifyUserAsync(oldUserName, userName);
            return Ok();
        }
        
        [HttpDelete("{userNameToDelete}")]
        public async Task<IActionResult> RemoveUser([FromRoute] string userNameToDelete)
        {
            await _logic.RemoveUserAsync(userNameToDelete);
            return Ok();
        }

        [Route("association")]
        [HttpPut()]
        public async Task<IActionResult> Association([FromBody] UserAndGameDTO association)
        {
            await _logic.AssociateGameAsync(association.Game,association.User);
            return Ok();
        }

        [Route("desassociation")]
        [HttpPut()]
        public async Task<IActionResult> Desassociation([FromBody] UserAndGameDTO desAssociation)
        {
            await _logic.DisassociateGameAsync(desAssociation.Game, desAssociation.User);
            return Ok();
        }
    }
}
