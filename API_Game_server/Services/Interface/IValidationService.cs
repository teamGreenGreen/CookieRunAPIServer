using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IValidationService
{
    public Task<string> GetUid(string Token);
}
