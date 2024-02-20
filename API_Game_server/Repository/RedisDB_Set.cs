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
        // String 자료형
        // key : value 쌍을 저장
        // value에 json타입을 저장하기도 한다.
        public async Task<bool> AddSetElement(string key, string member)
        {
            // Set 성공시 true
            bool result = await _db.SetAddAsync(key, member);
            return result;
        }
        // key에 대한 모든 멤버 조회
        // 싱글 스레드 환경에서 SetMembers를 하게되면 해당 순회를 하는 동안
        // 다른 요청이 Block되는 문제가 있다.
        // 이를 해결하기 위해서 SSCAN을 사용한다.
        public async Task<bool> GetSetIsMemberExist(string key, string value)
        {
            bool result = await _db.SetContainsAsync(key, value);
            return result;
        }
        public async Task<string[]> GetSetMembers(string key)
        {
            IAsyncEnumerable<RedisValue> result = _db.SetScanAsync(key);
            var temp = await result.ToListAsync();
            string[] members = new string[temp.Count];
            int i = 0;
            foreach (RedisValue value in temp)
            {
                members[i++] = value.ToString();
            }
            return members;
        }
        // key에 대한 멤버 수 조회
        public async Task<long> SizeOfSet(string key)
        {
            long result = await _db.SetLengthAsync(key);
            return result;
        }
        // key에 해당 멤버 삭제
        public async Task<bool> DeleteMember(string key, string member)
        {
            bool result = await _db.SetRemoveAsync(key, member);
            return result;
        }
    }
}
