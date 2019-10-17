using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public interface IARRService
    {
        public Task CreateSessionAsync(SessionDescriptor sessionDescriptor);
        Task<SessionsModel> GetSessionsAsync();
        Task StopServerAsync(string sessionId);
    }
}