using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService service;
        private readonly short maxDate = 32;
        public AttendanceController(AttendanceService _service)
        {
            service = _service;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("111");
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
        public async Task RequestAttendance()
        {
            // TODO : 출석 요청
        }
    }
}
