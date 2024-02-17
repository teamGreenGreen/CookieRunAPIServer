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
    public class MailDeleteController : ControllerBase
    {
        readonly MailService mailService;

        public MailDeleteController(MailService _mailService)
        {
            mailService = _mailService;
        }

        [HttpPost]
        public async Task<MailListRes> PostAsync(MailListReq req)
        {
            //응답 객체 생성
            MailListRes res = new();
            res.Result = await mailService.DeleteMailList(req);
            return res;
        }
    }
}
