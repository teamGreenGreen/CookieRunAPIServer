using System.ComponentModel;

//namespace API_Game_Server.Controllers.DTO
//{
public class RankUpdateReq
{
    public string UserName { get; set; }
    public double Score { get; set; }
}
public class RanksGetReq
{
    public int Page { get; set; }
}
public class RankGetReq
{
    public string UserName { get; set; }
}
//}
