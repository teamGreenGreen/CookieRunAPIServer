using API_Game_Server.Model.DTO;

namespace Auth_Server.Model.DTO;

public class LoginAccountReq
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class LoginAccountRes : ErrorCodeDTO
{
    public Int64 Uid { get; set; }
    public string AuthToken { get; set; }
}