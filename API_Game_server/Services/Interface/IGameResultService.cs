using API_Game_Server.Model.DAO;
using API_Game_Server.Model.DTO;
using API_Game_Server.Repository;
using Microsoft.VisualBasic.FileIO;

namespace API_Game_Server.Services.Interface;

public interface IGameResultService
{
    public Task<EErrorCode> ValidateRequestAsync(string sessionId, GameResultReq req);
    public Task<EErrorCode> GiveRewardsAsync(string sessionId, GameResultReq req, GameResultRes res);
    public int CalcMaxScore();
    public int CalcMoney();
    public int CalcExp();
    public Task UpdateUserInfoAsync(long uid, int newLevel, int newExp, int newMoney, int newDiamond, int newMaxScore, string userName);
    public void ReadGameData();
    public void ReadItemData();
    public void ReadUserLevelData();
    public void ReadCookieData();
}
