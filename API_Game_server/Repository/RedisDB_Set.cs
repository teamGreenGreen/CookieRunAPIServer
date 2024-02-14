using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using static System.Formats.Asn1.AsnWriter;

namespace API_Game_Server.Repository
{
    public partial class RedisDB
    {
        // String 자료형
        // value에 json타입을 저장하기도 한다.
        public async Task<bool> SetString(string key, string value)
        {
            // Set 성공시 true
            bool result = await _db.StringSetAsync(key, value);
            return result;
        }
        public async Task<RedisValue> GetString(string key)
        {
            // Get 실패시 nil
            RedisValue result = await _db.StringGetAsync(key);
            return result;
        }
    }
}
