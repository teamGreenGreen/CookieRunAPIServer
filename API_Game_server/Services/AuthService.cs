using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;

namespace API_Game_Server.Services;

public class AuthService : IAuthService
{
    private IGameDB gameDb;
    private readonly IRedisDB redisDb;
    private string authServerAddress;

    public AuthService(IConfiguration configuration, IGameDB gameDb, IRedisDB redisDb)
    {
        this.gameDb = gameDb;
        this.redisDb = redisDb;
        authServerAddress = configuration.GetSection("AuthServer").Value + "/VerifyToken"; 
    }

    public async Task<EErrorCode> VerifyTokenToAuthServer(Int64 userId, string authToken)
    {
        HttpClient client = new();

        // Post 요청을 보내고, Http 응답의 상태를 반환받음
        HttpResponseMessage response = await client.PostAsJsonAsync(authServerAddress, new { AuthToken = authToken, UserId = userId });
        if(response is null || response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return EErrorCode.AuthFailInvalidResponse;
        }

        // Http 요청에 대한 응답이 성공적으로 수신되었으면 Json 데이터를 읽어옴
        ErrorCodeDTO authResult = await response.Content.ReadFromJsonAsync<ErrorCodeDTO>();
        // 다른 api 서버에서 보낸 에러 코드는 여기서 알 수 없기 때문에 포괄적인 에러 코드로 처리
        if (authResult is null || authResult.Result != EErrorCode.None)
        {
            return EErrorCode.AuthFailInvalidResponse;
        }

        return EErrorCode.None;
    }

    // 유저가 존재하는지 확인
    public async Task<(EErrorCode, Int64)> VerifyUser(Int64 userId)
    {
        UserInfo userInfo = await gameDb.GetUserByUserId(userId);
        if (userInfo is null)
        {
            return (EErrorCode.LoginFailUserNotExist, 0);
        }

        return (EErrorCode.None, userInfo.Uid);
    }

    public async Task<(EErrorCode, string)> GenerateSessionId(Int64 uid)
    {
        string sessionId = Security.GenerateSessionId();

        // 발급한 세션ID redis에 추가

        // 1. gameDB에서 user_info 조회
        UserInfo userInfo = await gameDb.GetUserByUid(uid);
        if (userInfo == null) return (EErrorCode.LoginFailAddRedis, null);
        // 2. redis에 저장하기 위한 인스턴스 생성
        RedisUserInfo redis_info = GenerateSessionInfo(sessionId, userInfo);

        // 3. redis에 유저 정보(세션) 업데이트
        try
        {
            await redisDb.SetHash($"user_info:uid:{uid}", redis_info);
        }
        catch
        {
            return (EErrorCode.LoginFailAddRedis, null);
        }

        return (EErrorCode.None, sessionId);
    }

    public RedisUserInfo GenerateSessionInfo(string sessionId, UserInfo userInfo)
    {
        return new RedisUserInfo
        {
            SessionId = sessionId,
            UserName = userInfo.UserName,
            Level = userInfo.Level,
            Exp = userInfo.Exp,
            Money = userInfo.Money,
            Diamond = userInfo.Diamond,
            MaxScore = userInfo.MaxScore,
        };
    }
}
