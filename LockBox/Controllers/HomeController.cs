using LockBox.Commons.Models.Messages;
using LockBox.Commons.Models.Messages.User;
using LockBox.Commons.Services;
using LockBox.Models;
using LockBox.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace LockBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly SecurityHandler _jwtHandler;
        private readonly SendRequestService _sendRequestService;
        public HomeController(SecurityHandler jwtHandler, SendRequestService sendRequestService)
        {
            _jwtHandler = jwtHandler;
            _sendRequestService = sendRequestService;
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
            if (userRegisterRequest.Password.Length < 12)
            {
                ViewBag.Errors = "The password needs at least 12 characters";
                return View(userRegisterRequest);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = GetModelStateErrors();
                return View(userRegisterRequest);
            }

            string json = JsonConvert.SerializeObject(userRegisterRequest);
            string apiUrl = "https://localhost:44394/api/User/Register";
            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["Email"] = userRegisterRequest.Email;
                return RedirectToAction(nameof(EmailVerification));
            }
            if (apiResponse.Content != null)
            {
                ViewBag.Errors = GetFirstError(apiResponse).Result;
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
            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            UserEmailVerificationRequest fillFields = new UserEmailVerificationRequest
            {
                Email = request.Email
            };
            return View(fillFields);
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromForm]UserEmailVerificationRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            string apiUrl = "https://localhost:44394/api/User/VerifyCode";

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["MSG_S"] = "E-mail successfully verified!";
                return RedirectToAction(nameof(Login));
            }
            ViewBag.Errors = "An error ocurred, try again.";
            return View(request);
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

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                var token = await apiResponse.Content.ReadAsStringAsync();

                
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "User")
                    };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("UserCookies", token, cookieOptions);
                return RedirectToAction("Index", "Vault");
            }
            if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                TempData["Email"] = request.Email;
                return RedirectToAction("EmailVerification");
            }
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                ViewBag.Errors = "Invalid Login. Check your credentials";
                return View(request);
            }
            if (apiResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                ViewBag.Errors = "Too many requests. Try again later";
                return View(request);
            }
            ViewBag.Errors = GetFirstError(apiResponse).Result;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsRecruiter()
        {
            UserLoginRequest userRequest = new UserLoginRequest
            {
                Email = "wageyot233@javnoi.com",
                Password = "zTccI&U(E+[IyOURm1'~"
            };
            string json = JsonConvert.SerializeObject(userRequest);
            string apiUrl = "https://localhost:44394/api/User/Login";

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                var token = await apiResponse.Content.ReadAsStringAsync();


                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "User")
                    };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("UserCookies", token, cookieOptions);
                return RedirectToAction("Index", "Vault");
            }
            if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                TempData["Email"] = userRequest.Email;
                return RedirectToAction("EmailVerification");
            }
            if (apiResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                ViewBag.Errors = "Too many requests. Try again later";
                return View(userRequest);
            }
            ViewBag.Errors = GetFirstError(apiResponse).Result;
            return View(userRequest);
        }

        private string GetModelStateErrors()
        {
            var errors = ModelState.Values
                           .SelectMany(v => v.Errors)
                           .Select(e => e.ErrorMessage)
                           .ToList();

            return errors[0];
        }
        private async Task<string> GetFirstError(HttpResponseMessage responseMessage)
        {
            if(responseMessage.Content != null)
            {
                var errorResponseFromAPI = await responseMessage.Content.ReadAsStringAsync();
                ErrorResponse errorResponse = new ErrorResponse();
                errorResponse.Errors.Add(errorResponseFromAPI);

                return errorResponse.Errors[0];
            }
            return "Error";
        }
    }
}
