using API_Game_Server.Model.DTO;

namespace Auth_Server.Model.DTO;

public class VerifyTokenReq
{
    public string AuthToken { get; set; }
    public Int64 UserId { get; set; }
}

public class VerifyTokenRes : ErrorCodeDTO
{
}
