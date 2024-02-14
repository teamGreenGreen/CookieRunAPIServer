namespace Auth_Server.Model.DTO;

public class LoginAccountReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}

public class LoginAccountRes
{
    public Int64 Uid { get; set; }
    public string AuthToken { get; set; }
    public EErrorCode Result { get; set; } = EErrorCode.None;
}