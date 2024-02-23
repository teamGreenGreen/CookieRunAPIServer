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
    public class NowCookieController : ControllerBase
    {
        private readonly INowCookieService nowCookieService;
        public NowCookieController(INowCookieService _nowCookieService)
        {
            nowCookieService = _nowCookieService;
        }
        [HttpPost]
        public async Task<NowCookieRes> GetNowCookieId()
        {
            string sessionId = HttpContext.Features.Get<string>();
            NowCookieRes res = new NowCookieRes();
            (res.Result, res.NowCookieId) = await nowCookieService.NowCookieId(sessionId);
            return res;
        }
        [HttpPost("Edit")]
        public async Task<EditNowCookieRes> EditNowCookieId(EditNowCookieReq req)
        {
            string sessionId = HttpContext.Features.Get<string>();
            EditNowCookieRes res = new EditNowCookieRes();
            res.Result = await nowCookieService.EditNowCookieId(sessionId, req.CookieId);
            return res;
        }
    }
}