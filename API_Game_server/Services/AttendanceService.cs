using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using StackExchange.Redis;

namespace API_Game_Server.Services
{
    public class AttendanceService
    {
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;

        public AttendanceService(GameDB gameDB, RedisDB redisDB)
        {
            this.gameDB = gameDB;
            this.redisDB = redisDB;
        }

        // 서버에 기록되어있는 출석 시작 날짜 반환
        public async Task<AttendanceDateInfo> VerifyDatabaseData(DateTime now)
        {
            AttendanceDateInfo serverDate;
            string redisRes = await redisDB.GetString("attendance_date_info:start");
            if (redisRes == "")
            {
                // 해당 key가 존재하지 않는다.
                // GameDB에는 존재하는지 확인
                serverDate = await gameDB.GetServerDate();
                if (serverDate == null)
                {
                    // GameDB에도 없다. 오늘을 기준으로 한다.
                    await redisDB.SetString("attendance_date_info:start", now.ToString());
                    serverDate = await gameDB.SetServerDate(now);
                }
                else
                {
                    // GameDB에는 있다. Redis에 등록한다.
                    await redisDB.SetString("attendance_date_info:start", serverDate.AttendanceStartDate.ToString());
                }
            }
            else
            {
                // Redis cache hit!
                serverDate = new AttendanceDateInfo();
                serverDate.AttendanceStartDate = Convert.ToDateTime(redisRes);
            }
            return serverDate;
        }

        public async Task<int> GetTimeUntilNextRenewal(DateTime now, AttendanceDateInfo serverDate, int maxDate)
        {
            int remainDays;
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
                if (! await redisDB.SetString("attendance_date_info:start", remainder.ToString()))
                {
                    return -1;
                }
                await gameDB.SetServerDate(remainder);
                remainDays = duration.Days % maxDate;
            }
            return remainDays;
        }
        public async Task<int> GetUserAttendanceCount(AttendanceInfoReq req)
        {
            // 토큰으로 redis에서 유저 uid 조회
            string redisResUid = await redisDB.GetString(req.Tocken);
            if (long.TryParse(redisResUid, out long uid))
            {
                return -1;
            }
            AttendanceInfo attendanceInfo = await gameDB.GetUserAttendance(uid);
            if (attendanceInfo == null)
            {
                return -2;
            }
            return attendanceInfo.AttendanceCount;
        }
        public async Task<EErrorCode> GetRenewalAndAttendance(int maxDate, AttendanceInfoReq req, AttendanceInfoRes res)
        {
            DateTime now = DateTime.Now;
            AttendanceDateInfo serverDate = await VerifyDatabaseData(now);
            res.RemainDays = await GetTimeUntilNextRenewal(now, serverDate, maxDate);
            if (res.RemainDays == -1)
            {
                return EErrorCode.AttendanceFailSetString;
            }
            res.AttendanceCount = await GetUserAttendanceCount(req);
            if (res.AttendanceCount == -1)
            {
                return EErrorCode.AttendanceCountError;
            }
            if (res.AttendanceCount == -2)
            {
                return EErrorCode.AttendanceFailFindUser;
            }
            
            return EErrorCode.None;
        }

    }
}
