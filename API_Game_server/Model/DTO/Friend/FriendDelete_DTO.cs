using System.ComponentModel;
using API_Game_Server.Model.DAO;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터
    public class FriendDeleteReq
    {
        public string MyToken { get; set; }
        public string FriendName { get; set; }
    }
    // 응답 데이터
    public class FriendDeleteRes : ErrorCodeDTO
    {
        
    }
}