using API_Game_Server.Model.DAO;

namespace API_Game_Server.Model.DTO;

public class LoginReq
{
    public Int64 Uid {  get; set; }
    public string AuthToken { get; set; }
    public string UserName { get; set; }
}

public class LoginRes : ErrorCodeDTO
{
    public string SessionId { get; set; }
    public Int64 Uid { get; set;}
    public UserInfo UserInfo { get; set; }

}
