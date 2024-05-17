using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VaultController : ControllerBase
    {
        public VaultController()
        {
            
        }

        [HttpGet("GetAccounts")]
        public IActionResult GetAccounts()
        {
            return Ok();
        }
    }
}
