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
    public class MailDeleteController : ControllerBase
    {
        readonly IMailService mailService;

        public MailDeleteController(IMailService mailService)
        {
            this.mailService = mailService;
        }

        [HttpPost]
        public async Task<MailOpenRes> PostAsync(MailOpenReq req)
        {
            string sessionId = HttpContext.Features.Get<string>();

            //응답 객체 생성
            MailOpenRes res = new();
            res.Result = await mailService.DeleteMailAsync(sessionId, req);
            return res;
        }
    }
}
