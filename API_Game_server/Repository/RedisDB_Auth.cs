using API_Game_Server.Model.DAO;
using API_Game_Server.Repository.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace API_Game_Server.Repository;

public partial class RedisDB : IRedisDB
{
    private const string authUid = "user_info:uid:";

    public async Task<string> GetSessionIdAsync(Int64 uidKey)
    {
        string uid = authUid + uidKey.ToString();
        string[] uidValues = { "SessionId" };
        string[] sessionId = await GetHash(uid, uidValues);

        return sessionId[0];
    }
}