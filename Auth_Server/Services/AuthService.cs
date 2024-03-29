using API_Game_Server;
using Auth_Server.Model.DAO;
using Auth_Server.Model.DTO;
using Auth_Server.Repository;
using MySqlConnector;

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
        try
        {
            string saltValue = Security.GenerateSaltString();
            string hashingPassword = Security.GenerateHashingPassword(saltValue, password);

            int count = await accountDb.InsertAccountAsync(email, saltValue, hashingPassword);

            return count == 1 ? EErrorCode.None : EErrorCode.CreateAccountFail;
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1062)
            {
                return EErrorCode.CreateAccountFailDuplicate;
            }

            return EErrorCode.CreateAccountFail;
        }
        catch (Exception ex)
        {
            return EErrorCode.CreateAccountFail;
        }
    }

    public async Task<LoginAccountRes> VerifyUser(string email, string password)
    {
        LoginAccountRes response = new();

        Account account = await accountDb.GetAccount(email);
        if (account is null)
        {
            response.Result = EErrorCode.LoginFailUserNotExist;
            return response;
        }

        if(Security.GenerateHashingPassword(account.SaltValue, password) != account.Password)
        {
            response.Result = EErrorCode.LoginFailPwNotMatch;
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
