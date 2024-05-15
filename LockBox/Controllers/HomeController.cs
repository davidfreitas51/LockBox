using LockBox.Models;
using LockBox.Models.Messages;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace LockBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromForm]UserRegisterRequest userRegisterRequest)
        {
            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(userRegisterRequest);
                string apiUrl = "https://localhost:44394/api/User/Register";

                HttpClient httpClient = new HttpClient();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(EmailVerification));
                }
                else
                {
                    ViewBag.User = "Tristeza e memes";
                }
            }
            else
            {
                ViewBag.User = "Check your e-mail and password and try again";
            }

            return View(userRegisterRequest);
        }

        [HttpGet("EmailVerification")]
        public IActionResult EmailVerification()
        {
            return View();
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromForm]UserVerificationEmailRequest request)
        {
            request.Token = "";

            string json = JsonConvert.SerializeObject(request);
            string apiUrl = "https://localhost:44394/api/User/Register";

            HttpClient httpClient = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ViewBag.User = "Tristeza e memes";
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginRequest request)
        {

            string json = JsonConvert.SerializeObject(request);
            string apiUrl = "https://localhost:44394/api/User/Login";

            HttpClient httpClient = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Vault");

            }
            else
            {
                ViewBag.User = "Tristeza e memes";
                return View(request);
            }
        }
        public IActionResult Vault()
        {
            return View();
        }
    }
}
