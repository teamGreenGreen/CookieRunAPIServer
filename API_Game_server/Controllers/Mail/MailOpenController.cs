using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.AspNetCore.Mvc;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using API_Game_Server.Services;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailOpenController : ControllerBase
    {
        readonly MailService mailService;

        public MailOpenController(MailService mailService)
        {
            this.mailService = mailService;
        }

        [HttpPost]
        public async Task<MailRes> PostAsync(MailReq req)
        {
            //응답 객체 생성
            MailRes res = new();
            res.Result = await mailService.OpenMailAsync(req);
            return res;
        }
    }
}
