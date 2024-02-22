using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;
using Auth_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Server.Controllers;

[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService authService;

    public AccountController(IAuthService authService)
    {
        this.authService = authService;
    }

    [Route("Create")]
    [HttpPost]
    public async Task<CreateAccountRes> Create(CreateAccountReq request)
    {
        CreateAccountRes response = new();
        
        response.Result = await authService.CreateAccountAsync(request.Email, request.Password);

        return response;
    }

    [Route("Login")]
    [HttpPost]
    public async Task<LoginAccountRes> Login(LoginAccountReq request)
    {
        LoginAccountRes response = new();

        response = await authService.VerifyUser(request.Email, request.Password);

        return response;
    }
}
