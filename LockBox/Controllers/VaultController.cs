using Microsoft.AspNetCore.Mvc;

namespace LockBox.Controllers
{
    public class VaultController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
