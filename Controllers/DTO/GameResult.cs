// 요청 데이터
public class GameResultReq
{
    // 젤리, 돈, 플레이 시간
    public int UserId { get; set; }
    public TimeSpan? PlayTime  { get; set; }
    public Dictionary<int/*itemID*/, int/*count*/>? Items { get; set; }
    public uint Score { get; set; }
    public uint Money { get; set; }
    public int XPos { get; set; }
}

// 응답 데이터
public class GameResultRes
{
    public int Money { get; set; }
    public int Level { get; set; }
    public int Exp{ get; set; }
    public int MaxScore{ get; set; }
}

public class TestUserInfo
{
    public int Id { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Money{ get; set; }
    public int MaxScore { get; set; }
}
