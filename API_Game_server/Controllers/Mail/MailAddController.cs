using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.AspNetCore.Mvc;
using API_Game_Server.Repository;
using Microsoft.AspNetCore.Http;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;

namespace API_Game_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailAddController : ControllerBase
    {
        readonly MailService mailService;

        public MailAddController(MailService _mailService)
        {
            mailService = _mailService;
        }

        [HttpPost]
        public async Task<MailListRes> PostAsync(long _id, string _sender, string _content, int _count, bool _is_read, string _rewardType, DateTime _expiredAt)
        {
            {
                //응답 객체 생성
                MailListRes res = new();
                res.Result = await mailService.AddMailList(_id, _sender, _content, _count, _is_read, _rewardType, _expiredAt);
                return res;
            }
        }
    }
}
