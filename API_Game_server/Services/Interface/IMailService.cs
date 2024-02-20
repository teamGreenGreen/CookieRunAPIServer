using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface IMailService
{
    public Task<EErrorCode> GetMailListAsync(MailListReq req, MailListRes res);
    public Task AddMailAsync(long id, string sender, string content, int count, bool isRead, string rewardType, DateTime expiredAt);
    public Task<EErrorCode> OpenMailAsync(MailOpenReq req);
}
