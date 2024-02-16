using System.ComponentModel;

namespace API_Game_Server.Model.DAO
{
    public class AttendanceDateInfo
    {
        public DateTime AttendanceStartDate { get; set; }
    }
    public class AttendanceInfo
    {
        public long Uid { get; set; }
        public int AttendanceCount { get; set; }
        public DateTime AttendanceDate {  get; set; }
    }
}