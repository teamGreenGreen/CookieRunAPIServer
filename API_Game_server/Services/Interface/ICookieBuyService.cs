using API_Game_Server.Repository;

namespace API_Game_Server.Services.Interface;

public interface ICookieBuyService
{
    public int ReadCookieData(int cookieId);
    public Task<EErrorCode> CookieBuy(string sessionId, int cookieId);
}
