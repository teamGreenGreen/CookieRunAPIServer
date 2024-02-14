// 요청 데이터
public class AccountReq
{
    public string LoginId { get; set; }
    public string Password { get; set; }
}

// 응답 데이터
public class AccountRes
{
    public string LogindId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 모델
public class UserInfo
{
    public int Id { get; set; }
    public string LoginId { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
}
