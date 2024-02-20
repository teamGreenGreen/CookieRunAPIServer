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
        public async Task<string> GetUid(string Token)
        {
            string tokenKey = string.Format("token:{0}",Token);
            string myUid = await redisDB.GetString(tokenKey);
            return myUid;
        }
    }
}