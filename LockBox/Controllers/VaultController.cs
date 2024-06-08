using LockBox.Commons.Models;
using LockBox.Commons.Models.Messages.RegisteredAccount;
using LockBox.Models;
using LockBox.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace LockBox.Controllers
{
    [Authorize]
    public class VaultController : Controller
    {
        private readonly ISendRequestService _sendRequestService;
        public VaultController(ISendRequestService sendRequestService)
        {
            _sendRequestService = sendRequestService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            GetTokenAndUrlAccounts("Get", out string token, out string apiUrl);
            var request = new RATokenRequest { Token = token };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return View();
            }
            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                string responseContent = await apiResponse.Content.ReadAsStringAsync();
                List<RegisteredAccount> accountsList = JsonSerializer.Deserialize<List<RegisteredAccount>>(responseContent);
                return View(accountsList);
            }
            TempData["MSG_E"] = "An error occurred";
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> NewItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewItem([FromForm]RegisteredAccount account)
        {
            account.UserId = "0";
            ModelState.Remove("UserId");
            if (!ModelState.IsValid)
            {
                TempData["MSG_E"] = "An error occurred, check the fields";
                return View();
            }

            GetTokenAndUrlAccounts("Register", out string token, out string apiUrl);
            var request = new RATokenAccountRequest { Token = token, UserAccount = account };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["MSG_S"] = "Account successfully registered";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateItem(string id)
        {
            GetTokenAndUrlAccounts("GetById", out string token, out string apiUrl);
            var request = new RATokenAccIdRequest { Token = token, RAId = id, };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                string responseContent = await apiResponse.Content.ReadAsStringAsync();
                RegisteredAccount accountsList = JsonSerializer.Deserialize<RegisteredAccount>(responseContent);
                return View(accountsList);
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromForm] RegisteredAccount account)
        {
            account.UserId = "0";
            ModelState.Remove("UserId");
            if (!ModelState.IsValid)
            {
                TempData["MSG_E"] = "An error occurred, check the fields";
                return View();
            }

            GetTokenAndUrlAccounts("Update", out string token, out string apiUrl);
            var request = new RATokenAccountRequest { Token = token, UserAccount = account, };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["MSG_S"] = "Account successfully updated";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> DeleteItem([FromQuery]string id)
        {
            GetTokenAndUrlAccounts("DeleteAccount", out string token, out string apiUrl);
            RATokenAccIdRequest request = new RATokenAccIdRequest { Token = token,RAId = id, };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["MSG_S"] = "Account successfully deleted";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> CopyPassword([FromQuery] string id)
        {
            GetTokenAndUrlAccounts("CopyPassword", out string token, out string apiUrl);
            RATokenAccIdRequest request = new RATokenAccIdRequest {Token = token,RAId = id };
            var apiResponse = await _sendRequestService.PostRequest(request, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                var password = await apiResponse.Content.ReadAsStringAsync();
                TempData["MSG_Pass"] = password;
                TempData["MSG_S"] = "Password copied to your clipboard";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }

        private void GetTokenAndUrlAccounts(string endpoint, out string token, out string urlApi)
        {
            token = Request.Cookies["UserCookies"];
            urlApi = $"https://localhost:7070/api/accounts/{endpoint}";
        }
    }
}
