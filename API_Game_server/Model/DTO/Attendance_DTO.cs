namespace API_Game_Server.Model.DTO
{
    public class AttendanceInfoRes : ErrorCodeDTO
    {
        public int RemainDays { get; set; }
        public int AttendanceCount { get; set; }
    }
    public class AttendanceInfoReq
    {
        public string Token { get; set; }
    }
    public class AttendanceRes : ErrorCodeDTO
    {

    }
    public class AttendanceReq
    {
        public string Token { get; set; }
    }
}
