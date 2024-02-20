using API_Game_Server;
using Auth_Server.Model.DTO;
using Auth_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Server.Controllers;

[Route("[controller]")]
[ApiController]
public class VerifyTokenController : ControllerBase
{
    private readonly IAuthService authService;

    public VerifyTokenController(IAuthService authService)
    {
        this.authService = authService;
    }


    [HttpPost]
    public async Task<VerifyTokenRes> VerifyToken(VerifyTokenReq request)
    {
        VerifyTokenRes response = new();

        response.Result = await authService.VerifyToken(request.AuthToken, request.UserId);

        return response;
    }
}
