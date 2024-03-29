using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Model;
using StackExchange.Redis;
using System.Security.Cryptography.Xml;

namespace API_Game_Server.Repository;

public partial class GameDB : IGameDB
{
    public async Task<AttendanceInfo> GetUserAttendance(long uid)
    {
        AttendanceInfo res = await queryFactory.Query("ATTENDANCE_INFO")
            .Select("uid as Uid", "attendance_count as AttendanceCount", "attendance_date as AttendanceDate")
            .Where("uid", uid)
            .FirstOrDefaultAsync<AttendanceInfo>();
        return res;
    }
    public async Task<AttendanceInfo> SetUserAttendance(AttendanceInfo info, bool flag = true)
    {
        DateTime now = DateTime.Now;
        AttendanceInfo updateInfo = new AttendanceInfo ();
        if (flag)
        {
            updateInfo.Uid = info.Uid;
            updateInfo.AttendanceCount = info.AttendanceCount + 1;
            updateInfo.AttendanceDate = now;
        }
        else
        {
            updateInfo.Uid = info.Uid;
            updateInfo.AttendanceCount = 0;
            updateInfo.AttendanceDate = info.AttendanceDate;
        }

        int res = await queryFactory.Query("ATTENDANCE_INFO")
            .Where("uid", info.Uid)
            .UpdateAsync(new {
                attendance_count = updateInfo.AttendanceCount,
                attendance_date = updateInfo.AttendanceDate
            });
        if (res == null) return null;
        return updateInfo;
    }
    public async Task UpdateReward(UserInfo info, RewardItem reward)
    {
        // SQLKata 쿼리 생성
        if (reward.Name == "money")
        {
            await queryFactory.Query("USER_INFO").Where("uid", info.Uid).UpdateAsync(new
            {
                money = info.Money + reward.Count
            });
        }
        else if (reward.Name == "diamond")
        {
            await queryFactory.Query("USER_INFO").Where("uid", info.Uid).UpdateAsync(new
            {
                diamond = info.Diamond + reward.Count
            });
        }
    }
}