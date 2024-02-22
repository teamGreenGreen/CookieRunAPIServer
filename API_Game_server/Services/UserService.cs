using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;
using System.Text.Json;

namespace API_Game_Server.Services;

public class UserService : IUserService
{
    readonly IGameDB gameDb;
    readonly IRedisDB redisDb;

    public UserService(IGameDB gameDb, IRedisDB redisDb)
    {
        this.gameDb = gameDb;
        this.redisDb = redisDb;
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

    public async Task<EErrorCode> GetUserInfoBySessionId(string sessionId, UserInfoRes res)
    {
        try
        {
            // 레디스에서 데이터 가져오기
            string sessionIdKey = string.Format("user_info:session_id:{0}", sessionId);
            string redisFindResult = await redisDb.GetString(sessionIdKey);

            res.UserInfo = JsonSerializer.Deserialize<UserInfo>(redisFindResult);

            // 가져온 json 문자열을 userInfo로 역직렬화
            return EErrorCode.None;
        }
        catch
        {
            // TODO.김초원 : 로그아웃
            return EErrorCode.UserService_GetRedisUserInfoFail;
        }
    }
}
