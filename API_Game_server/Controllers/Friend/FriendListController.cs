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
    public class FriendListController : ControllerBase
    {
        private readonly IFriendListService friendListService;
        public FriendListController(IFriendListService _friendListService)
        {
            friendListService = _friendListService;
        }
        [HttpPost]
        public async Task<FriendListRes> GetFriendList()
        {
            string sessionId = HttpContext.Features.Get<string>();
            FriendListRes res = new FriendListRes();
            (res.Result, res.FriendList) = await friendListService.FriendList(sessionId);
            return res;
        }
    }
}