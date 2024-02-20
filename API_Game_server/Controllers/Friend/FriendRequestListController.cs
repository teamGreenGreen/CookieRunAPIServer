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
    public class FriendRequestListController : ControllerBase
    {
        private readonly IFriendRequestListService friendRequestListService;
        public FriendRequestListController(IFriendRequestListService _friendRequestListService)
        {
            friendRequestListService = _friendRequestListService;
        }
        [HttpPost]
        public async Task<FriendRequestListRes> GetFriendReqeustList(FriendRequestListReq req)
        {
            FriendRequestListRes res = new FriendRequestListRes();
            (res.Result, res.FriendRequestList) = await friendRequestListService.FriendRequestList(req.MyToken);
            return res;
        }
    }
}