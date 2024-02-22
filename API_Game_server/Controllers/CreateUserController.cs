using API_Game_Server.Model.DTO;
using API_Game_Server.Services;
using API_Game_Server.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_Game_Server.Controllers;

public class CreateUserController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IGameService gameService;
    private readonly IUserService userService;

    public CreateUserController(IAuthService authService, IGameService gameService, IUserService userService)
    {
        this.authService = authService;
        this.gameService = gameService;
        this.userService = userService;
    }

    // 게임 서버에 유저가 존재하지 않으면 만들어줘야 함
    [HttpPost]
    public async Task<CreateUserRes> CreateUser(CreateUserReq request)
    {
        CreateUserRes response = new();
        EErrorCode errorCode;

        (errorCode, response.Uid) = await gameService.CreateUserGameData(request.UserId, request.UserName);
        // 유저 생성이 잘 됐으면
        if (errorCode == EErrorCode.None)
        {
            // 신규 유저 보상 지급
            await gameService.CreateUserMailBox(response.Uid);
        }

        // 유저 데이터 로드
        (errorCode, response.UserInfo) = await userService.GetUserInfo(request.UserId);
        if (errorCode == EErrorCode.LoginFailUserNotExist)
        {
            response.Result = errorCode;
            return response;
        }

        return response;
    }
}
