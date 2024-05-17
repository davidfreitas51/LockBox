using LockBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LockBox.Controllers
{
    [Authorize]
    public class VaultController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userInfoJson = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/GetAccounts";

            var apiResponse = await SendPostRequest(userInfoJson, apiUrl);

            if (!apiResponse.IsSuccessStatusCode)
            {
                return View("Error");
            }

            string responseContent = await apiResponse.Content.ReadAsStringAsync();
            List<RegisteredAccount> accountsList = JsonSerializer.Deserialize<List<RegisteredAccount>>(responseContent);

            return View(accountsList);
        }
        private async Task<HttpResponseMessage> SendPostRequest(string jsonObj, string apiUrl)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(apiUrl, content);
        }
    }
}
