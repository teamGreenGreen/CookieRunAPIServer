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

    public async Task<EErrorCode> CreateUserMailBox(Int64 uid)
    {
        DateTime sevenDaysLater = DateTime.Now.AddDays(7);
        string sender = "운영자";
        string content = "신규 유저 보상";
        int count = 100;
        bool isRead = false;
        string rewardType = "diamond";

        try
        {
            await gameDb.AddMailAsync(uid, sender, content, count, isRead, rewardType, sevenDaysLater);
        }
        catch
        {
            return EErrorCode.MailService_CreateMailBoxFail;
        }

        return EErrorCode.None;
    }
}
