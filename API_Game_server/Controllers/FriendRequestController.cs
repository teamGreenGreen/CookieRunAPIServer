using API_Game_Server.Controllers.DTO;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// using ZLogger;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;
        private readonly ILogger<FriendRequestController> logger;
        public FriendRequestController(GameDB _gameDB, RedisDB _redisDB, ILogger<FriendRequestController> _logger)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            logger = _logger;
        }
        [HttpPost("addrequest")]
        public async Task<ActionResult> AddRequest(FriendAddReq req)
        {
            // 자기 자신의 이름인지 확인
            if (req.ToUserName == req.FromUserName) // 보내는 username과 받는 username이 동일하면 자기가 자기한테 친구 신청
            {
                return BadRequest(new { error = "자기 자신에게 친구 신청을 보낼 수 없습니다." });
            }

            // ToUserName의 유저가 존재하는지 확인
            FriendInfo myFriendInfo = await gameDB.GetFriendInfo(req.ToUserName);
            if(myFriendInfo == null)
            {
                return BadRequest(new { error = "존재하지 않는 유저입니다." });
            }

            // 이미 친구인지 확인
            FriendShipInfo friendshipInfo = await gameDB.GetFriendShipInfo(req.FromUserName, req.ToUserName);
            if(friendshipInfo != null)
            {
                return BadRequest(new { error = "이미 친구입니다." });
            }

            // 동일한 요청 존재하는지 확인
            FriendRequestInfo friendRequestInfo = await gameDB.GetFriendRequestInfo(req.FromUserName, req.ToUserName);
            if(friendRequestInfo != null)
            {
                return BadRequest(new { error = "이미 동일한 친구 신청이 존재합니다." });
            }

            // 최대 친구 수 초과 확인
            FriendCountInfo myFriendCount = await gameDB.GetMyFriendCountInfo(req.FromUserName);
            if(myFriendCount.FriendCount >= 50)
            {
                return BadRequest(new { error = "친구 수 제한을 초과하여 친구 신청을 할 수 없습니다."});
            }

            // 역방향 신청 존재하는지 확인
            

            // 신청이 완료된 경우
            await gameDB.InsertFriendRequest(req.FromUserName, req.ToUserName);
            return Ok(new {Message = "친구 신청이 성공적으로 처리되었습니다."});
        }
    }
}