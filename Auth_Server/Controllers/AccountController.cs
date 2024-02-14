using Auth_Server.Model.DTO;
using Auth_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountDB accountDb;

    public AccountController(AccountDB accountDb)
    {
        this.accountDb = accountDb;
    }

    [Route("Create")]
    [HttpPost]
    public async Task<CreateAccountRes> Create(CreateAccountReq request)
    {
        CreateAccountRes response = new();

        response.Result = await accountDb.CreateAccountAsync(request.AccountName, request.Password);

        return response;
    }

    [Route("Login")]
    [HttpPost]
    public async Task<LoginAccountRes> Login(LoginAccountReq request)
    {
        LoginAccountRes response = new();
        (response.Result, response.Uid) = await accountDb.VerifyUser(request.AccountName, request.Password);
        response.AuthToken = "1234"; // 해쉬 생성하는 함수 추가 예정

        return response;
    }
}
