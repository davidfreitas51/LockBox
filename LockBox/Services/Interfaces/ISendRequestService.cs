namespace LockBox.Services.Interfaces
{
    public interface ISendRequestService
    {
        Task<HttpResponseMessage> PostRequest<T>(T obj, string apiUrl);
    }
}
