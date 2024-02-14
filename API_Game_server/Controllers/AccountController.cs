using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using API_Game_Server.Repository;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountDB accountDB;

    public AccountController(AccountDB _accountDB)
    {
        accountDB = _accountDB;
    }

    [HttpGet("{Id}")]
    public async Task<AccountRes> Get(int id)
    {
        AccountRes response = new AccountRes();

        UserInfo userInfo = await accountDB.GetUserInfo(id);
        response.LogindId = userInfo.Login_Id;
        response.CreatedAt = userInfo.CreatedAt;

        return response;
    }

    [HttpPost]
    public async Task<AccountRes> Post(AccountReq request)
    {
        AccountRes response = new AccountRes();

        using (MySqlConnection connection = await Database.GetMySqlConnetion())
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Login_Id = request.LoginId;
            userInfo.Password = request.Password;

            await connection.QueryFirstOrDefaultAsync<int>(
                "INSERT INTO Account(LoginId,SaltValue,HashedPassword) Values(@Login_Id, @Password, @Password);", userInfo);
        }

        return response;
    }
}

