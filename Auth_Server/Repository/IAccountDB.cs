using Auth_Server.Model.DAO;
using SqlKata.Execution;

namespace Auth_Server.Repository;

public interface IAccountDB : IDisposable
{
    public Task<int> InsertAccountAsync(string email, string saltValue, string hashingPassword);
    public Task<Account> GetAccount(string email);
    public Task<string> GetSaltValue(Int64 userId);
}
