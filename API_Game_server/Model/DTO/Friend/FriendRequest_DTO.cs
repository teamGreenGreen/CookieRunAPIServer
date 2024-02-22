using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터
    public class FriendRequestReq
    {
        public string ToUserName { get; set; }
    }
    // 응답 데이터
    public class FriendRequestRes : ErrorCodeDTO
    {

    }
}