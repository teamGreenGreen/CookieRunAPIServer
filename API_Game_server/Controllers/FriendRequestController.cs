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
    public class FriendRequestController : ControllerBase
    {
        private readonly FriendRequestService friendRequestService;
        public FriendRequestController(FriendRequestService _friendRequestService)
        {
            friendRequestService = _friendRequestService;
        }
        [HttpPost("addrequest")]
        public async Task<FriendRequestRes> AddRequest(FriendRequestReq req)
        {
            FriendRequestRes res = new FriendRequestRes();
            res.Result = await friendRequestService.FriendRequest(req.MyToken, req.ToUserName);
            return res;
        }
    }
}