using LockBox.Commons.Models.Messages;
using LockBox.Commons.Models.Messages.User;
using LockBox.Commons.Services.Interfaces;
using LockBox.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace LockBox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISendRequestService _sendRequestService;
        private readonly ISecurityHandler _securityHandler;
        public HomeController(ISendRequestService sendRequestService, ISecurityHandler securityHandler)
        {
            _securityHandler = securityHandler;
            _sendRequestService = sendRequestService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            string abc;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromForm] UserRegisterRequest userRegisterRequest)
        {
            if (userRegisterRequest.Password == null)
            {
                ViewBag.Errors = "You need to have a password";
                return View(userRegisterRequest);
            }
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

            string apiUrl = GetApiUrlUser("Register");
            var apiResponse = await _sendRequestService.PostRequest(userRegisterRequest, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["Email"] = userRegisterRequest.Email;
                return RedirectToAction(nameof(EmailVerification));
            }
            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                ViewBag.Errors = "User already exists!";
            }
            else if (apiResponse.Content != null)
            {
                try
                {
                    ViewBag.Errors = GetFirstError(apiResponse).Result;
                }
                catch
                {
                    ViewBag.Errors = "An error ocurred";
                }
            }
            return View(userRegisterRequest);
        }

        [HttpGet("EmailVerification")]
        public async Task<IActionResult> EmailVerification()
        {
            UserAskConfirmationEmail request = new UserAskConfirmationEmail();
            request.Email = TempData["Email"].ToString();

            string apiUrl = GetApiUrlUser("SendVerificationCode");
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            UserEmailVerificationRequest fillFields = new UserEmailVerificationRequest
            {
                Email = request.Email
            };
            return View(fillFields);
        }

        [HttpPost("EmailVerification")]
        public async Task<IActionResult> EmailVerification([FromForm] UserEmailVerificationRequest request)
        {
            string apiUrl = GetApiUrlUser("VerifyCode");
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

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
        public async Task<IActionResult> Login([FromForm] UserLoginRequest request)
        {
            string apiUrl = GetApiUrlUser("Login");
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

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

        [HttpGet]
        public async Task<IActionResult> LoginAsRecruiter()
        {
            UserLoginRequest userRequest = new UserLoginRequest
            {
                Email = "wageyot233@javnoi.com",
                Password = _securityHandler.DecryptAES("yFADLsEJ+vJ+ikdliB9CcZa8O+6Ya/Z2NtyjLGHkdoE=")
            };
            string apiUrl = GetApiUrlUser("LoginRecruiter");

            var apiResponse = await _sendRequestService.PostRequest(userRequest, apiUrl);

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
            try
            {
                ViewBag.Errors = GetFirstError(apiResponse).Result;
            }
            catch
            {
                return RedirectToAction(nameof(LoginAsRecruiter));
            }
            return RedirectToAction("Login", userRequest);
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
            if (responseMessage.Content != null)
            {
                var errorResponseFromAPI = await responseMessage.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorResponseFromAPI);

                return errorResponse.Errors.FirstOrDefault() ?? "Error";
            }
            return "Error";
        }
        public string GetApiUrlUser(string endpoint)
        {
            string apiUrl = $"https://localhost:7070/api/user/{endpoint}";

            return apiUrl;
        }
    }
}