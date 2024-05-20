using LockBox.Commons.Models;
using LockBox.Models;
using LockBox.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace LockBox.Controllers
{
    [Authorize]
    public class VaultController : Controller
    {
        private readonly SendRequestService _sendRequestService;
        public VaultController(SendRequestService sendRequestService)
        {
            _sendRequestService = sendRequestService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string token = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/accounts/Get";

            var apiResponse = await _sendRequestService.PostRequest(token, apiUrl);

            if (apiResponse.StatusCode == HttpStatusCode.NotFound)
            {
                List<RegisteredAccount> lista = new List<RegisteredAccount>();
                RegisteredAccount acc = new RegisteredAccount
                {
                    Title = "Título teste",
                    Password = "Senha Teste",
                    Username = "Email teste"
                };
                lista.Add(acc);
                return View(lista);
            }

            string responseContent = await apiResponse.Content.ReadAsStringAsync();
            List<RegisteredAccount> accountsList = JsonSerializer.Deserialize<List<RegisteredAccount>>(responseContent);

            if (accountsList != null && accountsList.Count > 0)
            {
                return View(accountsList);
            }
            else
            {
                return View((object)null);
            }   
        }

        

        [HttpGet]
        public async Task<IActionResult> NewItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewItem([FromForm]RegisteredAccount account)
        {
            ModelState.Remove("UserId");
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "An error occurred";
                return View();
            }

            string token = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/accounts/Register";

            RARequest request = new RARequest
            {
                Token = token,
                UserAccount = account,
            };
            var json = JsonSerializer.Serialize(request);

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                ViewBag.Success = "Account successfully registered";
                return RedirectToAction("Index");
            }
            ViewBag.Error = "An error occurred";
            return RedirectToAction("Index");
        }
    }
}
