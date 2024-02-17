using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services;

public class UserService
{
    readonly GameDB gameDb;

    public UserService(GameDB gameDb)
    {
        this.gameDb = gameDb;
    }

    public async Task<(EErrorCode, UserInfo)> GetUserInfo(Int64 uid)
    {
        UserInfo userInfo = await gameDb.GetUserByUserId(uid);
        if (userInfo is null)
        {
            return (EErrorCode.LoginFailUserNotExist, null);
        }

        return (EErrorCode.None, userInfo);
    }
}
