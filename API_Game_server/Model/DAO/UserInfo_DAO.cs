namespace API_Game_Server.Model.DAO;

public class UserInfo
{
    public Int64 InfoId { get; set; }
    public Int64 Uid { get; set; }
    public string UserName { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Money { get; set; }
    public int MaxScore { get; set; }
    public int AcquiredCookieId { get; set; }
    public int diamond { get; set; }
}
