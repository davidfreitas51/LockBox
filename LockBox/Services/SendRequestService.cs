using System.Text;

namespace LockBox.Services
{
    public class SendRequestService
    {
        public async Task<HttpResponseMessage> PostRequest(string jsonObj, string apiUrl)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(apiUrl, content);
        }
    }
}
