using System.Text;
using System.Text.Json;

namespace LockBox.Services
{
    public class SendRequestService
    {
        public async Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl)
        {
            using HttpClient httpClient = new HttpClient();
            string jsonObj = JsonSerializer.Serialize(obj);
            var content = new StringContent(jsonObj, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(apiUrl, content);
        }
    }
}