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
            string token = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/accounts/Get";

            RAGetByUserRequest request = new RAGetByUserRequest
            {
                Token = token
            };
            string requestJson = JsonSerializer.Serialize(request);

            var apiResponse = await _sendRequestService.PostRequest(requestJson, apiUrl);

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
            return View((object)null);
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
                TempData["MSG_E"] = "An error occurred";
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
                TempData["MSG_S"] = "Account successfully registered";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateItem(string id)
        {
            string token = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/accounts/GetById";

            RAGetByIdRequest request = new RAGetByIdRequest
            {
                Token = token,
                RAId = id,
            };
            var json = JsonSerializer.Serialize(request);

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

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
                TempData["MSG_E"] = "An error occurred";
                return View();
            }

            string token = Request.Cookies["UserCookies"];
            string apiUrl = "https://localhost:44394/api/accounts/Update";

            RARequest request = new RARequest
            {
                Token = token,
                UserAccount = account,
            };
            var json = JsonSerializer.Serialize(request);

            var apiResponse = await _sendRequestService.PostRequest(json, apiUrl);

            if (apiResponse.IsSuccessStatusCode)
            {
                TempData["MSG_S"] = "Account successfully updated";
                return RedirectToAction("Index");
            }
            TempData["MSG_E"] = "An error occurred";
            return RedirectToAction("Index");
        }
    }
}
