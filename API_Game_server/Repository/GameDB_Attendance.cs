using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using System.Data;

namespace API_Game_Server.Repository;

public partial class GameDB : IDisposable
{
    public async Task<AttendanceInfo> GetUserAttendance(long uid)
    {
        return await queryFactory.Query("ATTENDANCE_INFO")
            .Select("uid as Uid, attendance_count as AttendanceCount, attendance_count as AttendanceDate")
            .Where("uid",uid)
            .FirstOrDefaultAsync<AttendanceInfo>();
    }
    public async Task<AttendanceInfo> SetUserAttendance(AttendanceInfo info, bool flag = true)
    {
        DateTime now = DateTime.Now;
        AttendanceInfo updateInfo = new AttendanceInfo ();
        if (flag)
        {
            updateInfo.AttendanceCount = info.AttendanceCount + 1;
            updateInfo.AttendanceDate = now;
        }
        else
        {
            updateInfo.AttendanceCount = 0;
            updateInfo.AttendanceDate = info.AttendanceDate;
        }

        int res = await queryFactory.Query("ATTENDANCE_INFO")
            .Where("uid", info.Uid)
            .UpdateAsync(new {
                attendance_count = updateInfo.AttendanceDate,
                attendance_date = updateInfo.AttendanceDate
            });
        if (res == null) return null;
        return updateInfo;
    }
    public async Task UpdateReward(ResultUserInfo info, int count)
    {
        // SQLKata 쿼리 생성
        await queryFactory.Query("USER_INFO").Where("uid", info.Uid).UpdateAsync(new
        {
            money = info.Money + count
        });
    }
}