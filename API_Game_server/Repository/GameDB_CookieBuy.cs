using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using System.Data;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Model;

namespace API_Game_Server.Repository;

public partial class GameDB : IGameDB
{
    public async Task UpdateCookieAndDiamond(long uid, int myDiamond, int cost, int newAcquiredCookieId)
    {
        await queryFactory.Query("USER_INFO").Where("uid", uid).UpdateAsync(new
        {
            diamond = myDiamond - cost,
            acquired_cookie_id = newAcquiredCookieId
        });
    }
}