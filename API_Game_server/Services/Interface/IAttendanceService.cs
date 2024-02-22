using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model;
using API_Game_Server.Repository;
using Microsoft.VisualBasic.FileIO;

namespace API_Game_Server.Services.Interface;

public interface IAttendanceService
{
    public Task<AttendanceDateInfo> VerifyDatabaseData(DateTime now);
    public Task<(int, EErrorCode)> GetTimeUntilNextRenewal(DateTime now, AttendanceDateInfo serverDate, int maxDate);
    public Task<(int, EErrorCode)> GetUserAttendanceCount(AttendanceReq req, DateTime AttendanceStartDate);
    public Task<EErrorCode> GetRenewalAndAttendance(int maxDate, AttendanceReq req, AttendanceRes res);
    public DateTime? ReadData();
    public DateTime WriteDate(DateTime date);
    public Task<AttendanceInfo> GetAttInfo(AttendanceReq req);
    public Task<AttendanceInfo> HasAttended(AttendanceReq req);
    public Task<RewardItem> SearchReward(int count);
    public Task<EErrorCode> GiveAndUpdateReward(AttendanceInfo info, RewardItem rewardItem);
    public Task<EErrorCode> RequestAttendance(AttendanceReq req, AttendanceRes res);
}
