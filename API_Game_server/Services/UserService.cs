using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;

namespace API_Game_Server.Services;

public class UserService : IUserService
{
    readonly IGameDB gameDb;

    public UserService(IGameDB gameDb)
    {
        this.gameDb = gameDb;
    }

    public async Task<(EErrorCode, UserInfo)> GetUserInfo(Int64 userId)
    {
        UserInfo userInfo = await gameDb.GetUserByUserId(userId);
        if (userInfo is null)
        {
            return (EErrorCode.LoginFailUserNotExist, null);
        }

        return (EErrorCode.None, userInfo);
    }
}
