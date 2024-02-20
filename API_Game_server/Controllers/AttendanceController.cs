using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService service;
        private readonly short maxDate = 32;
        public AttendanceController(IAttendanceService _service)
        {
            service = _service;
        }
        // 유저가 출석부를 요청 -> 갱신까지 남은 날과 현재까지 출석한 수를 반환
        [HttpPost("get")]
        public async Task<AttendanceInfoRes> GetAttendanceInfo(AttendanceInfoReq req)
        {
            AttendanceInfoRes res = new AttendanceInfoRes();
            res.Result = await service.GetRenewalAndAttendance(maxDate, req, res);
            return res;
        }
        [HttpPost("request")]
        public async Task<AttendanceRes> RequestAttendance(AttendanceReq req)
        {
            AttendanceRes res = new AttendanceRes();
            res.Result = await service.RequestAttendance(req, res);
            return res;
        }
    }
}
