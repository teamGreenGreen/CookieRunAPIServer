using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    public class RankUpdateReq
    {
        public string? UserName { get; set; }
        public double Score { get; set; }
    }
    public class RanksLoadReq
    {
        public int Page { get; set; }
        public int PlayerNum { get; set; }
    }
    public class RanksLoadRes : ErrorCodeDTO
    {
        public string[]? Ranks { get; set; }
    }
    public class  RankGetRes : ErrorCodeDTO
    {
        public string Rank { get; set; }
    }
    public class RankSizeRes : ErrorCodeDTO
    {
        public long Size { get; set; }
    }
}
