namespace API_Game_Server.Model.DTO
{
    public class AttendanceRes : ErrorCodeDTO
    {
        public int RemainDays { get; set; }
        public int AttendanceCount { get; set; }
        public string[] Rewards { get; set; }
    }
}
