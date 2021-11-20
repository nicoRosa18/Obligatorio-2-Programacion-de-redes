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
        public async Task<ActionResult<tipoUsuario>> Get()
        {
            //int test = await _logic.TestMethodAsync();
            return Ok(test);
        }

        [HttpPost("{newName}")]
        public async Task<> NewUser([FromRoute] string newUserName)
        {

        }

        [HttpPut("{oldUserName}")]
        public async Task<> ModifyUser([FromRoute] string oldUserName, [FromBody] )
        {

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
