using API_Game_Server.Model.DAO;
using System.ComponentModel.DataAnnotations;

namespace API_Game_Server.Model.DTO;

public class CreateUserReq
{
    [Required]
    public Int64 UserId { get; set; }
    [Required]
    public string UserName { get; set; }
}

public class CreateUserRes : ErrorCodeDTO
{
    [Required]
    public string SessionId { get; set; }
    [Required]
    public Int64 Uid { get; set; }
    public UserInfo UserInfo { get; set; }
}
