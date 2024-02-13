using API_Game_Server.Controllers.DTO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private readonly RedisDB _db;
        public RankController(RedisDB repository)
        {
            _db = repository;
        }
        // 유저 닉네임을 인자로 넘기면, 등수를 알 수 있다.
        [HttpGet("{UserName}")]
        public async Task<ActionResult> Get(string UserName)
        {
            var result = await _db.GetZsetRank("rank", UserName);
            return Ok((result+1).ToString());
        }
        // 유저의 점수를 업로드하면, Redis의 Sorted Set에 정렬된다.
        // 이후 해당 유저의 등수를 반환한다.
        [HttpPost]
        public async Task<ActionResult> Post(RankReq req)
        {
            await _db.SetZset("rank", req.UserName, req.Score);
            var result = await Get(req.UserName);
            return result;
        }
    }
}
