using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using static System.Formats.Asn1.AsnWriter;

namespace API_Game_Server.Repository
{
    public partial class RedisDB
    {
        public async Task SetHash<T>(string key, T obj) where T : class
        {
            var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            HashEntry[] Entries = new HashEntry[fields.Length];
            int i = 0;
            foreach (var field in fields)
            {
                Entries[i++] = new HashEntry(field.Name, field.GetValue(obj).ToString());
            }
            // 반환값 X
            await _db.HashSetAsync(key, Entries);
        }
        public async Task<string[]> GetHash(string key, string[] Items)
        {
            // string 배열을 인자로 받으면,
            // 해당 데이터를 RedisValue[] 자료형으로 변환하여 HashGetAsync에 전달한다.
            var Entries = new RedisValue[Items.Length];
            int i = 0;
            foreach (var Item in Items)
            {
                Entries[i++] = new RedisValue(Item);
            }
            RedisValue[] result = await _db.HashGetAsync(key,Entries);
            if (result == null) return null;
            if (result.Length == 0) return new string[0];
            // RedisValue[] 자료형을 string[] 으로 변환하여 반환한다.
            return Array.ConvertAll(result, x => (string)x);
        }
        public async Task<bool> ClearHash(string key)
        {
            bool result = await _db.KeyDeleteAsync(key);
            return result;
        }
    }
}
