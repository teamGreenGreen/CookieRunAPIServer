using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace API_Game_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RankController : ControllerBase
    {
        private readonly RedisDB _db;
        public RankController(RedisDB repository)
        {
            _db = repository;
        }
        // 유저 닉네임을 인자로 넘기면, 등수를 알 수 있다.
        [HttpPost("user")]
        public async Task<ActionResult> GetRank(RankGetReq req)
        {
            var result = await _db.GetZsetRank("rank", req.UserName);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok((result+1).ToString());
        }
        // 유저의 점수를 업로드하면, Redis의 Sorted Set에 정렬된다.
        // 이후 해당 유저의 등수를 반환한다.
        // 실제로 이 method가 호출될 일은 없다
        [HttpPost("update")]
        public async Task UpdateRank(RankUpdateReq req)
        {
            await _db.SetZset("rank", req.UserName, req.Score);
        }
        // 조회하고자 하는 page를 인자로 넘기면
        // page에 해당하는 랭커들을 조회하여 string[] 데이터로 반환한다.
        [HttpPost("total-rank")]
        public async Task<ActionResult> LoadRanks(RanksGetReq req)
        {
            RedisValue[] result = await _db.GetZsetRanks("rank", req.Page);
            if (result.Length == 0)
            {
                return BadRequest();
            }
            return Ok(Array.ConvertAll(result, x => (string)x));
        }

        // 전체 랭크의 개수를 반환한다.
        // 클라이언트가 전체 랭킹을 조회할 때, 마지막 페이지를 명시할 때 사용된다.
        [HttpPost("size-rank")]
        public async Task<ActionResult> SizeRank()
        {
            long result = await _db.GetZsetSize("rank");
            if (result == 0)
                return BadRequest();
            return Ok(result);
        }

        // rank를 key로 하는 zset의 모든 데이터 삭제
        [HttpDelete]
        public async Task<ActionResult> ClearRank()
        {
            long result = await _db.ClearZset("rank");
            return Ok(result);
        }

        [HttpPost("hash-set")]
        public async Task SetHashTest(TestUserInfo TestUserInfo)
        {
            string token = "1";
            await _db.SetHash(token, TestUserInfo);
        }

        [HttpPost("hash-get")]
        public async Task<ActionResult<RedisValue>> GetHashTest()
        {
            string token = "1";
            List<string> strings = new List<string>(){ "Id", "Level", "UserName" };
            RedisValue[]  result = await _db.GetHash(token, strings);
            return Ok(result);
        }
    }
}
