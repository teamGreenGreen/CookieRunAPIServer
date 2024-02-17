namespace Auth_Server.Model.DAO;

public class Account
{
    public Int64 UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string SaltValue { get; set; }
    public string CreateAt { get; set; }
}
