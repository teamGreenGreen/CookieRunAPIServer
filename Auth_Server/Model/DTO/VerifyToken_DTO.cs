using API_Game_Server.Model.DTO;
using System.ComponentModel.DataAnnotations;

namespace Auth_Server.Model.DTO;

public class VerifyTokenReq
{
    [Required]
    public string AuthToken { get; set; }
    [Required]
    public Int64 UserId { get; set; }
}

public class VerifyTokenRes : ErrorCodeDTO
{
}
