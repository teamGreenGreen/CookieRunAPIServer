using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;

namespace Auth_Server.Services;

public class AuthService
{
    private readonly AccountDB accountDb;

    public AuthService(AccountDB accountDb)
    {
        this.accountDb = accountDb;
    }

    public async Task<EErrorCode> CreateAccountAsync(string userName, string password)
    {
        string saltValue = Security.GenerateSaltString();
        string hashingPassword = Security.GenerateHashingPassword(saltValue, password);

        int count = await accountDb.InsertAccountAsync(userName, saltValue, hashingPassword);

        return count == 1 ? EErrorCode.None : EErrorCode.CreateAccountFail;
    }

    public async Task<LoginAccountRes> VerifyUser(string userName, string password)
    {
        LoginAccountRes response = new();

        Account account = await accountDb.GetAccount(userName, password);
        if (account is null)
        {
            response.Result = EErrorCode.LoginFailUserNotExist;
            return response;
        }

        response.AuthToken = Security.GenerateAuthToken(account.SaltValue, response.Uid);
        response.Uid = account.Uid;

        return response;
    }

    public async Task<EErrorCode> VerifyToken(string authToken, Int64 uid)
    {
        string saltValue = await accountDb.GetSaltValue(uid);

        if (authToken == Security.GenerateAuthToken(saltValue, uid))
            return EErrorCode.None;
        else
            return EErrorCode.VerifyTokenFail;
    }

}
