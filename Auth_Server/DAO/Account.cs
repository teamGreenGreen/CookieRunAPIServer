namespace Auth_Server.DAO;

public class Account
{
    public Int64 Uid { get; set; }
    public string AccountName { get; set; }
    public string Password { get; set; }
    public string SaltValue { get; set; }
    public string CreateAt { get; set; }
}
