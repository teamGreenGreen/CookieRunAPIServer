using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IUserService
{
    public Task<(EErrorCode, UserInfo)> GetUserInfo(Int64 userId);
    public Task<EErrorCode> GetUserInfoBySessionId(string sessionId, UserInfoRes res);
}
