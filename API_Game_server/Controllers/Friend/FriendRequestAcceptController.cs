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
    public class FriendRequestAcceptController : ControllerBase
    {
        private readonly IFriendRequestAcceptService friendRequestAcceptService;
        public FriendRequestAcceptController(IFriendRequestAcceptService _friendRequestAcceptService)
        {
            friendRequestAcceptService = _friendRequestAcceptService;
        }
        [HttpPost]
        public async Task<FriendRequestAcceptRes> AddRequest(FriendRequestAcceptReq req)
        {
            FriendRequestAcceptRes res = new FriendRequestAcceptRes();
            res.Result = await friendRequestAcceptService.FriendRequestAccept(req.RequestId);
            return res;
        }
    }
}