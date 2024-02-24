using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Resources;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API_Game_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService service;
        private readonly short maxDate = 19;
        public AttendanceController(IAttendanceService _service)
        {
            service = _service;
        }
        [HttpPost("request")]
        public async Task<AttendanceRes> RequestAttendance()
        {
            string sessionId = HttpContext.Features.Get<string>();

            AttendanceRes res = new AttendanceRes();
            res.Rewards = CalendarReward.Instance.rewards;
            // 갱신까지 남은 날과 현재까지 출석한 수 검색
            res.Result = await service.GetRenewalAndAttendance(maxDate, sessionId, res);
            if (res.Result != EErrorCode.None)
            {
                return res;
            }
            // 출석 진행
            res.Result = await service.RequestAttendance(sessionId, res);
            return res;
        }
    }
}
