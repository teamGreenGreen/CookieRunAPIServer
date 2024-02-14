namespace Auth_Server.Model.DTO;

public class CreateAccountReq
{
    public string AccountName { get; set; }
    public string Password { get; set; }
}

public class CreateAccountRes
{
    public EErrorCode Result { get; set; } = EErrorCode.None;
}