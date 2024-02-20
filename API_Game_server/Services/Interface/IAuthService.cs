using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IAuthService
{
    public Task<EErrorCode> VerifyTokenToAuthServer(Int64 userId, string authToken);
    public Task<(EErrorCode, Int64)> VerifyUser(Int64 userId);
    public Task<(EErrorCode, string)> GenerateSessionId(Int64 uid);
    public RedisUserInfo GenerateSessionInfo(string sessionId, UserInfo userInfo);
}
