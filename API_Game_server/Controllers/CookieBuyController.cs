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
    public class CookieBuyController : ControllerBase
    {
        private readonly ICookieBuyService cookieBuyService;
        public CookieBuyController(ICookieBuyService _cookieBuyService)
        {
            cookieBuyService = _cookieBuyService;
        }
        [HttpPost]
        public async Task<CookieBuyRes> CookieBuy(CookieBuyReq req)
        {
            string sessionId = HttpContext.Features.Get<string>();
            CookieBuyRes res = new CookieBuyRes();
            res.Result = await cookieBuyService.CookieBuy(sessionId, req.CookieId);
            return res;
        }
    }
}