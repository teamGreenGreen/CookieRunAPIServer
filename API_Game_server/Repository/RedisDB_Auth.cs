using API_Game_Server.Model.DAO;
using API_Game_Server.Repository.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace API_Game_Server.Repository;

public partial class RedisDB : IRedisDB
{
    private const string authUid = "user_info:session_id:";

    public async Task<bool> ExistSessionIdAsync(string sessionId)
    {
        string key = authUid + sessionId;

        string redisSessionId = await GetString(key);

        if (redisSessionId == "")
        {
            return false;
        }

        return true;
    }
}