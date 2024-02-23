using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;

namespace API_Game_Server.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IRedisDB redisDB;
        public ValidationService(IRedisDB _redisDB)
        {
            redisDB = _redisDB;
        }
        public async Task<long> GetUid(string sessionId)
        {
            UserInfo userInfo = await redisDB.GetString<UserInfo>($"user_info:session_id:{sessionId}");
            long myUid = userInfo.Uid;
            return myUid;
        }
    }
}