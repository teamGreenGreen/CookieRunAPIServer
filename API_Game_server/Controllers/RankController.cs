using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;
using API_Game_Server.Repository.Interface;

namespace API_Game_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RankController : ControllerBase
    {
        private readonly IRankService service;
        private readonly IRedisDB redis;
        public RankController(IRankService rankService, IRedisDB reids)
        {
            this.service = rankService;
            this.redis = reids;
        }
        // 토큰을 인자로 넘기면, 등수를 알 수 있다.
        [HttpPost("user")]
        public async Task<RankGetRes> GetRank(RankGetReq req)
        {
            RankGetRes res = new RankGetRes();
            res.Result = await service.GetRank(req, res);
            return res;
        }
        // 조회하고자 하는 page를 인자로 넘기면
        // page에 해당하는 랭커들을 조회하여 string[] 데이터로 반환한다.
        [HttpPost("total-rank")]
        public async Task<RanksLoadRes> LoadRanks(RanksLoadReq req)
        {
            RanksLoadRes res = new RanksLoadRes();
            res.Result = await service.LoadRanks(req, res);
            return res;
        }
        // 전체 랭크의 개수를 반환한다.
        // 클라이언트가 전체 랭킹을 조회할 때, 마지막 페이지를 명시하기위해 사용된다.
        [HttpPost("size-rank")]
        public async Task<RankSizeRes> SizeRank()
        {
            RankSizeRes res = new RankSizeRes();
            res.Result = await service.GetSizeOfRanks(res);
            return res;
        }

        [HttpPost("test")]
        public async Task SetJsonTest()
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Level = 1;
            userInfo.UserName = "test";
            userInfo.Exp = 12;
            userInfo.UserId = 1;
            userInfo.Money = 1;
            await redis.SetString<UserInfo>("user_info:session_id:123",userInfo);
        }

        [HttpPost("test2")]
        public async Task<UserInfo> GetJsonTest()
        {
            return await redis.GetString<UserInfo>("user_info:session_id:123");
        }
    }
}
