using API_Game_Server.Model;
using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Resources;
using API_Game_Server.Services.Interface;
using Microsoft.VisualBasic.FileIO;
using StackExchange.Redis;
using System.Runtime;

namespace API_Game_Server.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGameDB gameDB;
        private readonly IRedisDB redisDB;
        private readonly IValidationService validation;
        public AttendanceService(IGameDB gameDB, IRedisDB redisDB, IValidationService validation)
        {
            this.gameDB = gameDB;
            this.redisDB = redisDB;
            this.validation = validation;
        }

        public async Task<AttendanceDateInfo> VerifyDatabaseData(DateTime now)
        {
            DateTime? serverDate;
            string redisRes = await redisDB.GetString("attendance_date_info:start");
            if (redisRes == "")
            {
                // 해당 key가 존재하지 않는다.
                // GameDB에는 존재하는지 확인
                serverDate = ReadData();
                if (serverDate == null)
                {
                    // GameDB에도 없다. 오늘을 기준으로 한다.
                    await redisDB.SetString("attendance_date_info:start", now.ToString());
                    serverDate = WriteDate(now);
                }
                else
                {
                    // GameDB에는 있다. Redis에 등록한다.
                    await redisDB.SetString("attendance_date_info:start", serverDate.ToString());
                }
            }
            else
            {
                // Redis cache hit!
                serverDate = Convert.ToDateTime(redisRes);
            }
            AttendanceDateInfo result = new AttendanceDateInfo() { AttendanceStartDate = (DateTime)serverDate };
            return result;
        }
        public async Task<(int,EErrorCode)> GetTimeUntilNextRenewal(DateTime now, AttendanceDateInfo serverDate, int maxDate)
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
                if (!await redisDB.SetString("attendance_date_info:start", remainder.ToString()))
                {
                    return (-1,EErrorCode.AttendanceFailSetString);
                }
                WriteDate(remainder);
                remainDays = maxDate - duration.Days % maxDate;
            }
            return (remainDays,EErrorCode.None);
        }
        public async Task<(int, EErrorCode)> GetUserAttendanceCount(AttendanceReq req, DateTime AttendanceStartDate)
        {
            string redisResUid = await validation.GetUid(req.Token);
            if (long.TryParse(redisResUid, out long uid))
            {
                return (-1,EErrorCode.AttendanceCountError);
            }
            AttendanceInfo attendanceInfo = await gameDB.GetUserAttendance(uid);
            if (attendanceInfo == null)
            {
                return (-2, EErrorCode.AttendanceFailFindUser);
            }
            if((attendanceInfo.AttendanceDate - AttendanceStartDate).Days < 0)
            {
                AttendanceInfo resInfo = await gameDB.SetUserAttendance(attendanceInfo, false);
                if (resInfo == null)
                {
                    return (-3,EErrorCode.AttendanceUpdateFail);
                }
            }
            return (attendanceInfo.AttendanceCount, EErrorCode.None);
        }
        public async Task<EErrorCode> GetRenewalAndAttendance(int maxDate, AttendanceReq req, AttendanceRes res)
        {
            DateTime now = DateTime.Now;
            AttendanceDateInfo serverDate = await VerifyDatabaseData(now);

            var getNextTime = await GetTimeUntilNextRenewal(now, serverDate, maxDate);
            if (getNextTime.Item2 != EErrorCode.None)
            {
                return getNextTime.Item2;
            }
            res.RemainDays = getNextTime.Item1;

            var getAttendanceCount = await GetUserAttendanceCount(req, serverDate.AttendanceStartDate);
            if (getAttendanceCount.Item2 != EErrorCode.None)
            {
                return getAttendanceCount.Item2;
            }
            res.AttendanceCount = getAttendanceCount.Item1;
            return EErrorCode.None;
        }
        public DateTime? ReadData()
        {
            string filePath = "./Resources/AttendanceDate.csv";
            // 파일이 존재하지 않으면 null 반환
            if (!File.Exists(filePath))
            {
                return null;
            }
            // 파일이 존재한다면 파일 읽기
            string row = "";
            using (TextFieldParser parser = new TextFieldParser(filePath))
                // csv에 저장된 날짜 읽기
                row = parser.ReadLine();
            // 데이터가 있으면 반환
            if (row != null)
            {
                return Convert.ToDateTime(row);
            }
            // 비어있으면 null 반환
            return null;
        }
        public DateTime WriteDate(DateTime date)
        {
            string filePath = "./Resources/AttendanceDate.csv";
            File.WriteAllText(filePath, date.ToString());
            return date;
        }
        public async Task<AttendanceInfo> GetAttInfo(AttendanceReq req)
        {
            // 유저 출석 정보 가져오기
            string redisUid = await validation.GetUid(req.Token);
            if (!long.TryParse(redisUid, out long uid))
            {
                return null;
            }
            return await gameDB.GetUserAttendance(uid);
        }
        public async Task<AttendanceInfo> HasAttended(AttendanceReq req)
        {
            // 유저 출석 정보와 오늘 날짜 비교
            AttendanceInfo attInfo = await GetAttInfo(req);
            if (attInfo == null)
            {
                return null;  // 해당 유저 정보가 없을 때
            }
            if ((DateTime.Now - attInfo.AttendanceDate).Days == 0)
            {
                return null;   // 이미 오늘 출석을 했을 때
            }
            return attInfo;
        }
        public async Task<RewardItem> SearchReward(int count)
        {
            // 보상 검색
            RewardItem res = new RewardItem();
            string reward = CalendarReward.Instance.rewards[count];
            res.Name = reward.Split(":")[0];
            res.Count = Int32.Parse(reward.Split(":")[1]);
            return res;
        }
        public async Task<EErrorCode> GiveAndUpdateReward(AttendanceInfo info, RewardItem rewardItem)
        {
            UserInfo user = await gameDB.GetUserByUid(info.Uid);
            if (user == null)
            {
                return EErrorCode.NotExistUserDoingReward;
            }

            await gameDB.UpdateReward(user, rewardItem);

            return EErrorCode.None;
        }
        public async Task<EErrorCode> RequestAttendance(AttendanceReq req, AttendanceRes res)
        {
            // 1. 해당 유저가 출석을 할 수 있는 상태인지 확인
            AttendanceInfo attInfo = await HasAttended(req);
            if (attInfo == null)
            {
                return EErrorCode.AttendanceReqFail;
            }
            // 2. 출석을 할 수 있다면 attendance_count와 attendance_date를 업데이트
            AttendanceInfo resInfo = await gameDB.SetUserAttendance(attInfo);
            if (resInfo == null)
            {
                return EErrorCode.AttendanceUpdateFail;
            }
            // 3. 보상 검색
            RewardItem rewardItem = await SearchReward(resInfo.AttendanceCount);
            // 4. 보상 지급
            EErrorCode code = await GiveAndUpdateReward(resInfo, rewardItem);
            if (code != EErrorCode.None)
            {
                return code;
            }
            // 5. res 반환 - EErrorCode.None
            return EErrorCode.None;
        }
    }
}
