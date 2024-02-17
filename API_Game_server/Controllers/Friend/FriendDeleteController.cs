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
    public class FriendDeleteController : ControllerBase
    {
        private readonly FriendDeleteService friendDeleteService;
        public FriendDeleteController(FriendDeleteService _friendDeleteService)
        {
            friendDeleteService = _friendDeleteService;
        }
        [HttpPost]
        public async Task<FriendDeleteRes> FriendDelete(FriendDeleteReq req)
        {
            FriendDeleteRes res = new FriendDeleteRes();
            res.Result = await friendDeleteService.FriendDelete(req.MyToken, req.FriendName);
            return res;
        }
    }
}