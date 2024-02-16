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
    public class FriendRequestDenyController : ControllerBase
    {
        private readonly FriendRequestDenyService friendRequestDenyService;
        public FriendRequestDenyController(FriendRequestDenyService _friendRequestDenyService)
        {
            friendRequestDenyService = _friendRequestDenyService;
        }
        [HttpPost]
        public async Task<FriendRequestDenyRes> DenyRequest(FriendRequestDenyReq req)
        {
            FriendRequestDenyRes res = new FriendRequestDenyRes();
            res.Result = await friendRequestDenyService.FriendRequestDeny(req.MyToken, req.RequestId);
            return res;
        }
    }
}