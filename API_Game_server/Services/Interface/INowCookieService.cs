using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface INowCookieService
{
    public Task<(EErrorCode, int)> NowCookieId(string sessionId);
}
