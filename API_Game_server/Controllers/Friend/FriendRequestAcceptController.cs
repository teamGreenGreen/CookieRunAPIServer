using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendRequestAcceptController : ControllerBase
    {
        private readonly FriendRequestAcceptService friendRequestAcceptService;
        public FriendRequestAcceptController(FriendRequestAcceptService _friendRequestAcceptService)
        {
            friendRequestAcceptService = _friendRequestAcceptService;
        }
        [HttpPost("allow")]
        public async Task<FriendRequestAcceptRes> AddRequest(FriendRequestAcceptReq req)
        {
            FriendRequestAcceptRes res = new FriendRequestAcceptRes();
            res.Result = await friendRequestAcceptService.FriendRequestAccept(req.MyToken, req.RequestId);
            return res;
        }
    }
}