using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;

namespace Auth_Server.Services;

public class AuthService : IAuthService
{
    private readonly IAccountDB accountDb;

    public AuthService(IAccountDB accountDb)
    {
        this.accountDb = accountDb;
    }

    public async Task<EErrorCode> CreateAccountAsync(string email, string password)
    {
        string saltValue = Security.GenerateSaltString();
        string hashingPassword = Security.GenerateHashingPassword(saltValue, password);

        int count = await accountDb.InsertAccountAsync(email, saltValue, hashingPassword);

        return count == 1 ? EErrorCode.None : EErrorCode.CreateAccountFail;
    }

    public async Task<LoginAccountRes> VerifyUser(string email, string password)
    {
        LoginAccountRes response = new();

        Account account = await accountDb.GetAccount(email, password);
        if (account is null)
        {
            response.Result = EErrorCode.LoginFailUserNotExist;
            return response;
        }

        response.UserId = account.UserId;
        response.AuthToken = Security.GenerateAuthToken(account.SaltValue, response.UserId);

        return response;
    }

    public async Task<EErrorCode> VerifyToken(string authToken, Int64 userId)
    {
        string saltValue = await accountDb.GetSaltValue(userId);

        if (authToken == Security.GenerateAuthToken(saltValue, userId))
            return EErrorCode.None;
        else
            return EErrorCode.VerifyTokenFail;
    }

}
