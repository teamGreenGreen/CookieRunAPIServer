using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.AspNetCore.Mvc;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameResultController : ControllerBase
    {
        readonly GameResultService gameResultService;

        public GameResultController(GameResultService _gameResultService)
        {
            gameResultService = _gameResultService;
        }

        [HttpPost]
        public async Task<GameResultRes> PostAsync(GameResultReq req)
        {
            //응답 객체 생성
            GameResultRes res = new();
            res.Result = await gameResultService.ValidateResult(req);
            return res;
        }
    }
}