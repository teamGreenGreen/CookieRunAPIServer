using StackExchange.Redis;
using System.Reflection;

namespace API_Game_Server.Repository.Interface;

public interface IRedisDB
{
    public Task<bool> SetZset(string key, string member, double score);
    public Task<long?> GetZsetRank(string key, string member);
    public Task<SortedSetEntry[]> GetZsetRanks(string key, int page, int playerNum);
    public Task<long> GetZsetSize(string key);
    public Task<long> ClearZset(string key);
    public Task<string> GetSessionIdAsync(Int64 uidKey);
    public Task SetHash<T>(string key, T obj) where T : class;
    public Task<string[]> GetHash(string key, string[] Items);
    public Task<bool> ClearHash(string key);
    public Task<bool> AddSetElement(string key, string member);
    public Task<bool> GetSetIsMemberExist(string key, string value);
    public Task<string[]> GetSetMembers(string key);
    public Task<long> SizeOfSet(string key);
    public Task<bool> DeleteMember(string key, string member);
    public Task<bool> SetString(string key, string value);
    public Task SetString<T>(string key, T value);

    public Task<string> GetString(string key);
    public Task<T> GetString<T>(string key);
}
