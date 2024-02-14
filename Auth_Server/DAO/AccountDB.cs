namespace Auth_Server.DAO;

public class AccountDB
{
    public Int64 uid { get; set; }
    public string account_name { get; set; }
    public string password { get; set; }
    public string salt_value { get; set; }
    public string recent_login_time { get; set; }
}
