using API_Game_Server.Model.DTO;
using System.ComponentModel.DataAnnotations;

namespace Auth_Server.Model.DTO;

public class CreateAccountReq
{
    [Required]
    [StringLength(40, ErrorMessage = "Email is To Long"]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string Email { get; set; }
    [Required]
    [StringLength(30, ErrorMessage = "Password is To Long"]
    public string Password { get; set; }
}

public class CreateAccountRes : ErrorCodeDTO
{
}