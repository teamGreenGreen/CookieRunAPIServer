namespace Auth_Server.Model.DAO;

public class Account
{
    public long Uid { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SaltValue { get; set; }
    public string CreateAt { get; set; }
}
