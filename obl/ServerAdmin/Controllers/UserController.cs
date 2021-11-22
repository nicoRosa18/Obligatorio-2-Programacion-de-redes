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
        

        [HttpPost("{newUserName}")]
        public async Task<IActionResult> NewUser([FromRoute] string newUserName)
        {
            await _logic.AddUserAsync(newUserName);
            return Ok($"User added: {newUserName}");
        }

        [HttpPut("{oldUserName}")]
        public async Task<IActionResult> ModifyUser([FromRoute] string oldUserName, [FromBody] UserDTO user)
        {
            await _logic.ModifyUserAsync(oldUserName, user.Name);
            return Ok($"user {oldUserName} modified to {user.Name}");
        }
        
        [HttpDelete("{userNameToDelete}")]
        public async Task<IActionResult> RemoveUser([FromRoute] string userNameToDelete)
        {
            await _logic.RemoveUserAsync(userNameToDelete);
            return Ok($"User: {userNameToDelete} deleted successfully!");
        }

        [Route("association")]
        [HttpPut]
        public async Task<IActionResult> Association([FromBody] UserAndGameDTO association)
        {
            await _logic.AssociateGameAsync(association.Game,association.User);
            return Ok($"Game {association.Game} associated to user: {association.User}");
        }

        [Route("desassociation")]
        [HttpPut]
        public async Task<IActionResult> Desassociation([FromBody] UserAndGameDTO desAssociation)
        {
            await _logic.DisassociateGameAsync(desAssociation.Game, desAssociation.User);
            return Ok($"Game {desAssociation.Game} Disassociated to user: {desAssociation.User}");
        }
    }
}
