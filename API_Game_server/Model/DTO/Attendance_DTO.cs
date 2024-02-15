namespace API_Game_Server.Model.DTO
{
    public class AttendanceInfoRes
    {
        public int RemainDays { get; set; }
        public int AttendanceCount { get; set; }
    }
    public class AttendanceInfoReq
    {
        public string Tocken { get; set; }
    }
}
