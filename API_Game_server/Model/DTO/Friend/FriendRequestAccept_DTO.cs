using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터
    public class FriendRequestAcceptReq
    {
        public string MyToken { get; set; }
        public long RequestId { get; set; }
    }
    // 응답 데이터
    public class FriendRequestAcceptRes : ErrorCodeDTO
    {

    }
}