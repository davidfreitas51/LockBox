using LockBox.Commons.Models;
using LockBox.Commons.Models.Messages.RegisteredAccount;
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
            AppUser user = JsonSerializer.Deserialize<AppUser>(Request.Cookies["UserCookies"]);
            RAGetByUserRequest request = new RAGetByUserRequest
            {
                AppUser = user
            };
            string requestJson = JsonSerializer.Serialize(request);

            string apiUrl = "https://localhost:44394/api/accounts/Get";

            var apiResponse = await _sendRequestService.PostRequest(requestJson, apiUrl);

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
            AppUser user = JsonSerializer.Deserialize<AppUser>(Request.Cookies["UserCookies"]);
            account.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = "Fill correctly all the fields";
                return View();
            }
    
            string apiUrl = "https://localhost:44394/api/accounts/Get";

           // var apiResponse = await _sendRequestService.PostRequest(requestJson, apiUrl);
    
            return RedirectToAction("Index");
        }
    }
}
