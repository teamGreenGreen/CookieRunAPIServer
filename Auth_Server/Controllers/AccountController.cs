using Auth_Server.DTO;
using Auth_Server.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountDB accountDb;

    public AccountController(AccountDB accountDb)
    {
        this.accountDb = accountDb;
    }

    [Route("/Create")]
    [HttpPost]
    public async Task<CreateAccountRes> Create(CreateAccountReq registerRequest)
    {
        CreateAccountRes response = new();

        return response;
    }

    [Route("/Login")]
    [HttpPost]
    public async Task<LoginAccountRes> Create(LoginAccountReq registerRequest)
    {
        LoginAccountRes response = new();

        return response;
    }
}
