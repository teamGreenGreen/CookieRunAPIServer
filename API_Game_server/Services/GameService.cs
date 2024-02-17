using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services;

public class GameService
{
    private GameDB gameDb;

    public GameService(GameDB gameDb)
    {
        this.gameDb = gameDb;
    }

    public async Task<(EErrorCode, int)> CreateUserGameData(Int64 userId, string userName)
    {
        if(string.IsNullOrEmpty(userName))
        {
            return (EErrorCode.CreateUserFailEmptyNickname, 0);
        }

        UserInfo existUser = await gameDb.GetUserByUserName(userName);
        if (existUser is not null)
        {
            return (EErrorCode.CreateUserFailDuplicateNickname, 0);
        }

        return (EErrorCode.None, await gameDb.InsertUserGetId(userId, userName));
    }
}
