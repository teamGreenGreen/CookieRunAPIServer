using API_Game_Server.Model.DTO;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IGameService gameService;
    private readonly IUserService userService;

    public LoginController(IAuthService authService, IGameService gameService, IUserService userService)
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
        EErrorCode errorCode = await authService.VerifyTokenToAuthServer(request.UserId, request.AuthToken);
        if(errorCode != EErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        // 유저가 있는지 확인
        (errorCode, response.Uid) = await authService.VerifyUser(request.UserId);
        // 유저가 없으면 유저 생성
        if(errorCode == EErrorCode.LoginFailUserNotExist)
        {
            (errorCode, response.Uid) = await gameService.CreateUserGameData(request.UserId, request.UserName);

            // 유저 생성이 잘 됐으면
            if (errorCode == EErrorCode.None)
            {
                // 신규 유저 보상 지급
                await gameService.CreateUserMailBox(response.Uid);
            }
        }

        // 세션ID 발급, redis에 추가
        (errorCode, response.SessionId) = await authService.GenerateSessionId(response.Uid);
        if(errorCode == EErrorCode.LoginFailAddRedis)
        {
            response.Result = errorCode;
            return response;
        }
        
        // 유저 데이터 로드
        (errorCode, response.UserInfo) = await userService.GetUserInfo(request.UserId);
        if(errorCode == EErrorCode.LoginFailUserNotExist)
        {
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
