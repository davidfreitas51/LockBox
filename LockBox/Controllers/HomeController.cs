using LockBox.Commons.Models.Messages;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
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
            //TODO: Voltar a verificação de tamanho de senha
            /*
            if(userRegisterRequest.Password.Length < 12)
            {
                ViewBag.Errors = "The password needs at least 12 characters";
                return View(userRegisterRequest);
            }
             */

            if (ModelState.IsValid)
            {
                string json = JsonConvert.SerializeObject(userRegisterRequest);
                string apiUrl = "https://localhost:44394/api/User/Register";

                HttpClient httpClient = new HttpClient();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = userRegisterRequest.Email;
                    return RedirectToAction(nameof(EmailVerification));
                }
                else
                {
                    if (response.Content != null)
                    {
                        var errorResponseFromAPI = await response.Content.ReadAsStringAsync();
                        ErrorResponse errorResponse = new ErrorResponse();
                        errorResponse.Errors.Add(errorResponseFromAPI);
                        ViewBag.Errors = errorResponse.Errors[0];
                    }
                    return View(userRegisterRequest);
                }
            }
            else
            {
                var errors = ModelState.Values
                           .SelectMany(v => v.Errors)
                           .Select(e => e.ErrorMessage)
                           .ToList();

                ViewBag.Errors = errors[0];
            }

            return View(userRegisterRequest);
        }

        [HttpGet("EmailVerification")]
        public async Task<IActionResult> EmailVerification()
        {
            UserAskConfirmationEmail request = new UserAskConfirmationEmail();
            request.Email = TempData["Email"].ToString();

            string json = JsonConvert.SerializeObject(request);
            string apiUrl = "https://localhost:44394/api/User/SendVerificationCode";

            HttpClient httpClient = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, content);

            return View();
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromForm]UserEmailVerificationRequest request)
        {
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
                if (response.Content != null)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var errorObject = JsonConvert.DeserializeObject<string>(errorResponse);
                    ViewBag.Errors = errorObject;
                }
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromForm]UserLoginRequest request)
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
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                TempData["Email"] = request.Email;
                return RedirectToAction("EmailVerification");
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
