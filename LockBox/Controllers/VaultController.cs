using LockBox.Models;
using LockBox.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            string userInfoJson = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/GetAccounts";

            var apiResponse = await _sendRequestService.PostRequest(userInfoJson, apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View();
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
    }
}
