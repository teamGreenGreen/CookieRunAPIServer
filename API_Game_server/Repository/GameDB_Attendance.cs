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
            .Select("attendance_count as AttendanceCount, attendance_count as AttendanceDate")
            .Where("uid",uid)
            .FirstOrDefaultAsync<AttendanceInfo>();
    }
}