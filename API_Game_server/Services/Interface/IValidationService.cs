
namespace API_Game_Server.Services.Interface;

public interface IValidationService
{
    public Task<long> GetUid(string sessionId);
}
