using API_Game_Server.Model.DTO;
using API_Game_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly AuthService authService;

    public LoginController(AuthService authService)
    {
        this.authService = authService;
    }

    // 인증 서버에서 토큰 인증을 요청하고, 게임 서버에 로그인을 하고 유저 데이터를 불러온다.
    [HttpPost]
    public async Task<LoginRes> LoginAndLoadData(LoginReq request)
    {
        LoginRes response = new();

        // 인증 서버에 토큰 인증 요청
        EErrorCode errorCode = await authService.VerifyTokenToAuthServer(request.Uid, request.AuthToken);
        if(errorCode != EErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
