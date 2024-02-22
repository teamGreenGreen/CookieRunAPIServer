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
    public class FriendDeleteController : ControllerBase
    {
        private readonly IFriendDeleteService friendDeleteService;
        public FriendDeleteController(IFriendDeleteService _friendDeleteService)
        {
            friendDeleteService = _friendDeleteService;
        }
        [HttpPost]
        public async Task<FriendDeleteRes> FriendDelete(FriendDeleteReq req)
        {
            string sessionId = HttpContext.Features.Get<string>();
            FriendDeleteRes res = new FriendDeleteRes();
            res.Result = await friendDeleteService.FriendDelete(sessionId, req.FriendName);
            return res;
        }
    }
}