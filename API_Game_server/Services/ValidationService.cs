using API_Game_Server.Repository;

namespace API_Game_Server.Services
{
    public class ValidationService
    {
        private readonly RedisDB redisDB;
        public ValidationService(RedisDB _redisDB)
        {
            redisDB = _redisDB;
        }
        public async Task<string> GetUid(string Token)
        {
            string tokenKey = string.Format("token:{0}", Token);
            string myUid = await redisDB.GetString(tokenKey);
            return myUid;
        }
    }
}