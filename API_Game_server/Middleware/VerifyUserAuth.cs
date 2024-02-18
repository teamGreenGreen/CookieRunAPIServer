using API_Game_Server.Repository;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace API_Game_Server.Middleware;

public class VerifyUserAuth
{
    private readonly RedisDB redisDb;
    private readonly RequestDelegate next;

    public VerifyUserAuth(RequestDelegate next, RedisDB redisDb)
    {
        this.redisDb = redisDb;
        this.next = next;  
    }

    public async Task Invoke(HttpContext context)
    {
        string formString = context.Request.Path.Value;
        // 로그인 요청은 토큰 검사를 하지 않음
        if (string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0)
        {
            // 다음 미들웨어를 호출
            await next(context);

            return;
        }

        EErrorCode errorCode;
        string sessionId;
        string uid;

        (errorCode, sessionId) = GetSessionIdFromHeader(context);
        if(sessionId == null)
        {
            await ErrorResponse(context, StatusCodes.Status400BadRequest, errorCode);
        }

        // TODO : uid는 Int64? string? Redis에 키를 넣을 때 어떤 타입으로 넣을지 알아야 함
        (errorCode, uid) = GetUidFromHeader(context);
        if (uid == null)
        {
            await ErrorResponse(context, StatusCodes.Status400BadRequest, errorCode);
        }

        // TODO : Redis에서 uid를 키로 유저 인증 데이터(uid, sessionId) 불러옴
        // uid에 해당하는 데이터가 없으면 에러 반환 후 종료

        // TODO : sessionId가 일치하는지 검사

        await next(context);
    }

    private (EErrorCode, string) GetSessionIdFromHeader(HttpContext context)
    {
        StringValues sessionId;

        if(context.Request.Headers.TryGetValue("Authorization", out sessionId))
        {
            return (EErrorCode.None, sessionId);
        }

        return (EErrorCode.SessionIdNotProvided, null);
    }

    private (EErrorCode, string) GetUidFromHeader(HttpContext context)
    {
        StringValues uid;

        if (context.Request.Headers.TryGetValue("uid", out uid))
        {           
            return (EErrorCode.None, uid);
        }

        return (EErrorCode.UidNotProvided, null);
    }


    private async Task ErrorResponse(HttpContext context, int statusCode, EErrorCode errorCode)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errorJsonResponse = JsonSerializer.Serialize(new
        {
            result = errorCode
        });

        await context.Response.WriteAsync(errorJsonResponse);
    }
}
