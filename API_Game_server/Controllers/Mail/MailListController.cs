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

    public class MailListController : ControllerBase
    {
        readonly MailService mailService;

        public MailListController(MailService mailService)
        {
            this.mailService = mailService;
        }

        [HttpPost]
        public async Task<MailListRes> PostAsync(MailListReq req)
        {
            //응답 객체 생성
            MailListRes res = new();

            // 메일 리스트 불러오기
            res.Result = await mailService.GetMailListAsync(req, res);
            return res;
        }
    }
}
