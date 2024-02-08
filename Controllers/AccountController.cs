using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("{Id}")]
    public async Task<AccountRes> Get(int id)
    {
        AccountRes response = new AccountRes();

        using (MySqlConnection connection = await Database.GetMySqlConnetion())
        {
            UserInfo userInfo = await connection.QuerySingleOrDefaultAsync<UserInfo>(
                "SELECT LoginId, HashedPassword, CreatedAt FROM Account WHERE Id = @id", new { id = id } );

            response.LogindId = userInfo.LoginId;
            response.CreatedAt = userInfo.CreatedAt;
        }

        return response;
    }

    [HttpPost]
    public async Task<AccountRes> Post(AccountReq request)
    {
        AccountRes response = new AccountRes();

        using (MySqlConnection connection = await Database.GetMySqlConnetion())
        {
            UserInfo userInfo = new UserInfo();
            userInfo.LoginId = request.LoginId;
            userInfo.Password = request.Password;

            await connection.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Account(LoginId,SaltValue,HashedPassword) Values(@LoginId, @Password, @Password);", userInfo);
        }

        return response;
    }
}

