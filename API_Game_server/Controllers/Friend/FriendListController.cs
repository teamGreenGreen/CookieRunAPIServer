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
    public class FriendListController : ControllerBase
    {
        private readonly FriendListService friendListService;
        public FriendListController(FriendListService _friendListService)
        {
            friendListService = _friendListService;
        }
        [HttpPost]
        public async Task<FriendListRes> GetFriendList(FriendListReq req)
        {
            FriendListRes res = new FriendListRes();
            (res.Result, res.FriendList) = await friendListService.FriendList(req.MyToken);
            return res;
        }
    }
}