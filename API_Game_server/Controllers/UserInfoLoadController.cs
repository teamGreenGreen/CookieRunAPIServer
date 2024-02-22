using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.AspNetCore.Mvc;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class UserInfoLoadController : ControllerBase
    {
        readonly IUserService userService;

        public UserInfoLoadController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<UserInfoRes> PostAsync()
        {
            string sessionId = HttpContext.Features.Get<string>();

            // 응답 객체 생성
            UserInfoRes res = new();

            // 요청 검증
            res.Result = await userService.GetUserInfoBySessionId(sessionId, res);

            return res;
        }
    }
}
