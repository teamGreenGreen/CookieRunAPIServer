using API_Game_Server.Model.DTO;

namespace Auth_Server.Model.DTO;

public class CreateAccountReq
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class CreateAccountRes : ErrorCodeDTO
{
}