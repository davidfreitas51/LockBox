using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Models;
using LockBox.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace LockBox.Controllers
{
    [Authorize]
    public class VaultController : Controller
    {
        private readonly SendRequestService _sendRequestService;
        private readonly IHttpClientFactory _httpClientFactory;

        public VaultController(SendRequestService sendRequestService, IHttpClientFactory httpClientFactory)
        {
            _sendRequestService = sendRequestService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["UserCookies"];

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string apiUrl = "https://localhost:44394/api/Accounts/Get";

            var apiResponse = await client.GetAsync(apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
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
            else
            {
                return View("Error");
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
