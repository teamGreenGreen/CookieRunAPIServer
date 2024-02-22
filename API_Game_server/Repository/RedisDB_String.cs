using API_Game_Server.Repository.Interface;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace API_Game_Server.Repository
{
    public partial class RedisDB : IRedisDB
    {
        // String 자료형
        // key : value 쌍을 저장
        // value에 json타입을 저장하기도 한다.
        public async Task<bool> SetString(string key, string value)
        {
            // Set 성공시 true
            bool result = await _db.StringSetAsync(key, value);
            return result;
        }
        public async Task<string> GetString(string key)
        {
            // Get 실패시 ""
            RedisValue result = await _db.StringGetAsync(key);
            return result.ToString();
        }
        public async Task SetString<T>(string key, T instance)
        {
            string value = JsonSerializer.Serialize<T>(instance);
            await _db.StringSetAsync(key, value,new TimeSpan(7,0,0,0));
        }
        public async Task<T> GetString<T>(string key)
        {
            var getValue = await _db.StringGetAsync(key);
            T value = JsonSerializer.Deserialize<T>(getValue);
            return value;
        }
    }
}
