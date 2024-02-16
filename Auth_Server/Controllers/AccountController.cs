using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;
using Auth_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly AuthService authService;

    public AccountController(AuthService authService)
    {
        this.authService = authService;
    }

    [Route("Create")]
    [HttpPost]
    public async Task<CreateAccountRes> Create(CreateAccountReq request)
    {
        CreateAccountRes response = new();

        response.Result = await authService.CreateAccountAsync(request.UserName, request.Password);

        return response;
    }

    [Route("Login")]
    [HttpPost]
    public async Task<LoginAccountRes> Login(LoginAccountReq request)
    {
        LoginAccountRes response = new();

        response = await authService.VerifyUser(request.UserName, request.Password);

        return response;
    }
}
