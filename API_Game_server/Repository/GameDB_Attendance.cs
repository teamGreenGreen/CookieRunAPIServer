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
    public async Task<AttendanceDateInfo> GetServerDate()
    {
        return await queryFactory.Query("ATTENDANCE_DATE_INFO")
        .OrderByDesc("attendance_date_id")
        .Select("attendance_date_id as AttendanceDateId", "attendance_start_date as attendanceStartDate")
        .FirstOrDefaultAsync<AttendanceDateInfo>();
    }

    public async Task<AttendanceDateInfo> SetServerDate(DateTime date)
    {
        int id = await queryFactory.Query("ATTENDANCE_DATE_INFO").InsertAsync( new
        {
            attendance_start_date = date
        });
        return new AttendanceDateInfo() { AttendanceDateId = id, AttendanceStartDate = date };
    }

    public async Task<AttendanceInfo> GetUserAttendance(long uid)
    {
        return await queryFactory.Query("ATTENDANCE_INFO")
            .Select("attendance_count as AttendanceCount, attendance_count as AttendanceDate")
            .Where("uid",uid)
            .FirstOrDefaultAsync<AttendanceInfo>();
    }
}