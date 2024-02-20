using API_Game_Server.Repository.Interface;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using static System.Formats.Asn1.AsnWriter;

namespace API_Game_Server.Repository
{
    public partial class RedisDB : IRedisDB
    {
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
