using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using Microsoft.Extensions.Primitives;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace API_Game_Server.Middleware;

public class VerifyUserAuth
{
    private readonly IRedisDB redisDb;
    private readonly RequestDelegate next;

    public VerifyUserAuth(RequestDelegate next, IRedisDB redisDb)
    {
        this.redisDb = redisDb;
        this.next = next;  
    }

    public async Task Invoke(HttpContext context)
    {
        string formString = context.Request.Path.Value;
        // 로그인, 유저 생성 요청은 세션ID 검사를 하지 않음
        if (string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0 ||
            string.Compare(formString, "/CreateUser", StringComparison.OrdinalIgnoreCase) == 0)
        {
            // 다음 미들웨어를 호출
            await next(context);

            return;
        }

        EErrorCode errorCode;
        string sessionId;
        Int64 uid;

        // Http 헤더에서 sessionId를 가져옴
        (errorCode, sessionId) = GetSessionIdFromHeader(context);
        if(errorCode != EErrorCode.None)
        {
            await ErrorResponse(context, StatusCodes.Status400BadRequest, errorCode);
            return;
        }

        // Http 헤더에서 uid를 가져옴
        (errorCode, uid) = GetUidFromHeader(context);
        if (errorCode != EErrorCode.None)
        {
            await ErrorResponse(context, StatusCodes.Status400BadRequest, errorCode);
            return;
        }
        
        // redis에 sessionId가 존재하는지 확인
        bool existSessionId = await redisDb.ExistSessionIdAsync(sessionId);
        if (!existSessionId)
        {
            await ErrorResponse(context, StatusCodes.Status401Unauthorized, EErrorCode.SessionIdNotFound);
            return;
        }

        AuthInfo authInfo = new();
        authInfo.Uid = uid;
        authInfo.SessionId = sessionId;

        //context.Items[nameof(AuthInfo)] = authInfo;
        context.Features.Set<string>(authInfo.SessionId);

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

    private (EErrorCode, int) GetUidFromHeader(HttpContext context)
    {
        StringValues uid;

        if (context.Request.Headers.TryGetValue("Uid", out uid))
        {           
            return (EErrorCode.None, int.Parse(uid));
        }

        return (EErrorCode.UidNotProvided, 0);
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

    private bool IsValidSessionId(HttpContext context, string sessionId, string redisSessionId)
    {
        if (string.CompareOrdinal(sessionId, redisSessionId) == 0)
        {
            return true;
        }

        return false;
    }
}
