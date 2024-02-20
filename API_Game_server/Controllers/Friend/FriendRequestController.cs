using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API_Game_Server.Services.Interface;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService friendRequestService;
        public FriendRequestController(IFriendRequestService _friendRequestService)
        {
            friendRequestService = _friendRequestService;
        }
        [HttpPost]
        public async Task<FriendRequestRes> AddRequest(FriendRequestReq req)
        {
            FriendRequestRes res = new FriendRequestRes();
            res.Result = await friendRequestService.FriendRequest(req.MyToken, req.ToUserName);
            return res;
        }
    }
}