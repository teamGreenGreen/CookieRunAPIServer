using System.ComponentModel;

namespace API_Game_Server.Controllers.DTO
{
    public class RankReq
    {
        public string UserName {  get; set; }
        public double Score { get; set; }
    }
    public class RanksReq
    { 
        public int Page {  get; set; } 
    }
}
