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
    public class GameResultController : ControllerBase
    {
        readonly IGameResultService gameResultService;

        public GameResultController(IGameResultService gameResultService)
        {
            this.gameResultService = gameResultService;
        }

        [HttpPost]
        public async Task<GameResultRes> PostAsync(GameResultReq req)
        {
             string sessionId = HttpContext.Features.Get<string>();

            //응답 객체 생성
            GameResultRes res = new();

            // 요청 검증
            res.Result = await gameResultService.ValidateRequestAsync(sessionId, req);
            if (res.Result != EErrorCode.None)
                return res;

            // 게임 보상 지급
            res.Result = await gameResultService.GiveRewardsAsync(sessionId, req, res);
            return res;
        }
    }
}