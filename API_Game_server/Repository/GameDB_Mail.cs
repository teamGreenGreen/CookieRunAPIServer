using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using API_Game_Server.Repository.Interface;

namespace API_Game_Server.Repository;

public partial class GameDB : IGameDB
{
    public Task<IEnumerable<MailInfo>> GetMailListAsync(long id)
    {
        return queryFactory.Query("MAILBOX")
            .Select("mailbox_id as MailboxId", "uid", "is_read as IsRead", "sender", "content", "reward_type as RewardType", "count", "expired_at as ExpiredAt", "created_at as CreatedAt")
            .Where("uid", id)
            .Where("is_read", false)
            .Where("expired_at", ">=", DateTime.Now)
            .GetAsync<MailInfo>();
    }

    public Task AddMailAsync(long id, string sender, string content, int count, bool isRead, string rewardType, DateTime expiredAt)
    {
        return queryFactory.Query("MAILBOX").InsertAsync(new
        {
            uid = id,
            sender = sender,
            content = content,
            reward_type = rewardType,
            count = count,
            is_read = isRead,
            expired_at = expiredAt
        });
    }

    public Task<MailInfo> GetMailAsync(long id, int mailboxId)
    {
        return queryFactory.Query("MAILBOX")
        .Select("reward_type as RewardType", "count")
        .Where("mailbox_id", mailboxId)
        .Where("uid", id)
        .Where("is_read", false)    
        .FirstOrDefaultAsync<MailInfo>();
    }

    public Task UpdateMailAsync(long id, int mailboxId)
    {
        return queryFactory.Query("MAILBOX")
        .Where("mailbox_id", mailboxId)
        .Where("uid", id)
        .UpdateAsync(new
        {
            is_read = true
        });
    }
}