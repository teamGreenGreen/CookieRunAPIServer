using System.ComponentModel;

namespace API_Game_Server.Model.DAO
{
    public class AttendanceDateInfo
    {
        public long AttendanceDateId { get; set; }
        public DateTime AttendanceStartDate { get; set; }
    }
    public class AttendanceInfo
    {
        public int AttendanceCount { get; set; }
        public DateTime AttendanceDate {  get; set; }
    }
}