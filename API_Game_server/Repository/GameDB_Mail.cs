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

namespace API_Game_Server.Repository;

public partial class GameDB : IDisposable
{
    public async Task<IEnumerable<MailInfo>> GetMailList(long id)
    {
        IEnumerable<MailInfo> mailInfo = await queryFactory.Query("MAILBOX")
            .Select("mailbox_id as MailboxId", "uid", "is_read as IsRead", "sender", "content", "reward_type as RewardType", "count", "expired_at as ExpiredAt", "created_at as CreatedAt")
            .Where("uid", id)
            .Where("is_read", false)
            .Where("expired_at", ">=", DateTime.Now)
        .GetAsync<MailInfo>();

        return mailInfo;
    }

    public async Task<EErrorCode> AddMailList(long _id, string _sender, string _content, int _count, bool _is_read, string _rewardType, DateTime _expiredAt)
    {
        await queryFactory.Query("MAILBOX").InsertAsync(new
        {
            uid = _id,
            sender = _sender,
            content = _content,
            reward_type = _rewardType,
            count = _count,
            is_read = _is_read,
            expired_at = _expiredAt
        });

        return EErrorCode.None;
    }

    public async Task<EErrorCode> DeleteMailAndGetReward(long id, int mailboxId)
    {
        // 보상 주기
        MailInfo mailInfo = await queryFactory.Query("MAILBOX")
        .Select("reward_type as RewardType", "count")
        .Where("mailbox_id", mailboxId)
        .Where("uid", id)
        .Where("is_read", false)
        .FirstOrDefaultAsync<MailInfo>();

        ResultUserInfo userInfo = await queryFactory.Query("USER_INFO")
        .Select("uid", "level", "exp", "money", "max_score as MaxScore", "user_name as UserName")
        .Where("uid", id)
        .FirstOrDefaultAsync<ResultUserInfo>();

        // 읽음 표시 하고
        await queryFactory.Query("MAILBOX")
        .Where("mailbox_id", mailboxId)
        .Where("uid", id)
        .UpdateAsync(new
        {
            is_read = true
        });

        if (mailInfo.RewardType == "none")
        {
            return EErrorCode.None;
        }

        if (mailInfo.RewardType == "diamond")
        {
            // 보상하기
            await queryFactory.Query("USER_INFO")
            .Where("uid", id)
            .UpdateAsync(new
            {
                diamond = userInfo.Diamond + mailInfo.Count
            });
        }

        if (mailInfo.RewardType == "money")
        {
            // 보상하기
            await queryFactory.Query("USER_INFO")
            .Where("uid", id)
            .UpdateAsync(new
            {
                money = userInfo.Money + mailInfo.Count
            });
        }

        return EErrorCode.None;
    }
}