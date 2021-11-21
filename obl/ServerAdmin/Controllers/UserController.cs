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

        [HttpGet]
        public async Task<ActionResult<int>> Get()
        {
            //int test = await _logic.TestMethodAsync();
            return Ok();
        }

        [HttpPost("{newName}")]
        public async Task<IActionResult> NewUser([FromRoute] string newUserName)
        {
            return Ok();
        }

        [HttpPut("{oldUserName}")]
        public async Task<IActionResult> ModifyUser([FromRoute] string oldUserName, [FromBody] string userName)
        {
            return Ok();
        }
        
        [HttpDelete("{userNameToDelete}")]
        public async Task<IActionResult> RemoveUser([FromRoute] string userNameToDelete)
        {

            return Ok();
        }

        [Route("association")]
        [HttpPut()]
        public async Task<IActionResult> Association([FromRoute] UserAndGameDTO association)
        {

            return Ok();
        }

        [Route("desassociation")]
        [HttpPut()]
        public async Task<IActionResult> Desassociation([FromRoute] UserAndGameDTO desAssociation)
        {

            return Ok();
        }
    }
}
