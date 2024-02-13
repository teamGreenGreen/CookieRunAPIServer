using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using static System.Formats.Asn1.AsnWriter;

namespace API_Game_Server.Repository
{
    public class RedisDB
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisDB(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

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

        // Hash 자료형
        // HashEntry는 name, value가 한 쌍인 객체
        public async Task SetHash(string key, HashEntry[] entries)
        {
            await _db.HashSetAsync(key, entries);
        }
        public async Task<RedisValue[]> GetHash(string key, RedisValue[] hashFields)
        {
            RedisValue[] result = await _db.HashGetAsync(key, hashFields);
            return result;
        }

        // Sorted Sets 자료형
        public async Task<bool> SetZset(string key,  string member, double score)
        {
            // true : 기존에 없던 member
            // false : 기존에 존재한 member - 하지만 업데이트는 된다
            bool result = await _db.SortedSetAddAsync((RedisKey)key, (RedisValue)member, score);
            return result;
        }
        public async Task<long?> GetZsetRank(string key, string member)
        {
            // 해당 멤버가 없으면 null 반환
            // 있으면 랭크 반환 ( 1등은 0 반환 )
            long? result = await _db.SortedSetRankAsync((RedisKey)key, (RedisValue)member, Order.Descending);
            return result;
        }
        public async Task<RedisValue[]> GetZsetRanks(string key, int page)
        {
            long startNum = (page - 1) * 15;
            long endNum = (page) * 15 - 1;
            RedisValue[] result = await _db.SortedSetRangeByRankAsync(key, startNum, endNum, Order.Descending);
            return result;
        }
        public async Task<long> GetZsetSize(string key)
        {
            // 존재하지 않을 경우 0 반환
            long result = await _db.SortedSetLengthAsync(key);
            return result;
        }

        public async Task<long> ClearZset(string key)
        {
            long result = await _db.SortedSetRemoveRangeByRankAsync(key, 0, -1);
            return result;
        }
    }
}
