using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;

namespace Auth_Server.Services;

public interface IAuthService
{
    public Task<EErrorCode> CreateAccountAsync(string email, string password);
    public Task<LoginAccountRes> VerifyUser(string email, string password);
    public Task<EErrorCode> VerifyToken(string authToken, Int64 userId);
}
