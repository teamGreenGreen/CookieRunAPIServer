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
    private readonly GameService gameService;
    private readonly UserService userService;

    public LoginController(AuthService authService, GameService gameService, UserService userService)
    {
        this.authService = authService;
        this.gameService = gameService;
        this.userService = userService;
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

        // 유저가 있는지 확인
        (errorCode, response.Uid) = await authService.VerifyUser(request.Uid);
        // 유저가 없으면 유저 생성
        if(errorCode == EErrorCode.LoginFailUserNotExist)
        {
            (errorCode, response.Uid) = await gameService.CreateUserGameData(request.Uid, request.UserName);
        }

        // 세션ID 발급, redis에 추가
        (errorCode, response.SessionId) = await authService.GenerateSessionId(request.Uid);
        if(errorCode == EErrorCode.LoginFailAddRedis)
        {
            response.Result = errorCode;
            return response;
        }
        
        // 유저 데이터 로드
        (errorCode, response.UserInfo) = await userService.GetUserInfo(request.Uid);
        if(errorCode == EErrorCode.LoginFailUserNotExist)
        {
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
