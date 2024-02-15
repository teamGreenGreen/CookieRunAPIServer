using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService service;
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;
        private readonly short maxDate = 32;
        public AttendanceController(GameDB _gameDB, RedisDB _redisDB, AttendanceService _service)
        {
            service = _service;
            gameDB = _gameDB;
            redisDB = _redisDB;
        }
        // 유저가 출석부를 요청
        [HttpPost("get")]
        public async Task<AttendanceInfoRes> GetAttendanceInfo(AttendanceInfoReq req)
        {
            AttendanceInfoRes res = new AttendanceInfoRes();
            string redisRes;
            DateTime now = DateTime.Now;
            AttendanceDateInfo serverDate;
            int remainDays;
            int attendanceCount = 0;

            #region DB 신뢰성 확인
            redisRes = await redisDB.GetString("attendance_date_info:start");
            if (redisRes == "")
            {
                // 해당 key가 존재하지 않는다.
                // GameDB에는 존재하는지 확인
                serverDate = await gameDB.GetServerDate();
                // GameDB에도 없다. 오늘을 기준으로 한다.
                if (serverDate == null)
                {
                    await redisDB.SetString("attendance_date_info:start", now.ToString());
                    serverDate = await gameDB.SetServerDate(now);
                }
                // GameDB에는 있다. Redis에 등록한다.
                else
                {
                    await redisDB.SetString("attendance_date_info:start", serverDate.AttendanceStartDate.ToString());
                }
            }
            else
            {
                serverDate = new AttendanceDateInfo();
                serverDate.AttendanceStartDate = Convert.ToDateTime(redisRes);
            }
            #endregion

            #region 출석 갱신 여부 확인
            // 날짜 계산
            TimeSpan duration = now - serverDate.AttendanceStartDate;
            // 갱신이 필요없는 기간
            if (duration.Days < maxDate)
            {
                remainDays = maxDate - duration.Days;
            }
            // 갱신이 필요한 기간
            else
            {
                DateTime remainder = now.Subtract(new TimeSpan(duration.Days % maxDate));
                await redisDB.SetString("attendance_date_info:start", remainder.ToString());
                await gameDB.SetServerDate(remainder);
                remainDays = duration.Days % maxDate;
            }
            #endregion

            #region 유저 출석 현황 확인
            // 토큰으로 redis에서 유저 uid 조회
            string redisResUid =  await redisDB.GetString(req.Tocken);
            if (long.TryParse(redisResUid, out long uid))
            {
                // TODO : 에러반환
                return null;
            }
            AttendanceInfo attendanceInfo = await gameDB.GetUserAttendance(uid);
            attendanceCount = attendanceInfo.AttendanceCount;
            #endregion

            return new AttendanceInfoRes
            {
                RemainDays = remainDays,
                AttendanceCount = attendanceCount
            };
        }
        [HttpPost("request")]
        public async Task RequestAttendance()
        {
            // TODO : 출석 요청
        }
    }
}
