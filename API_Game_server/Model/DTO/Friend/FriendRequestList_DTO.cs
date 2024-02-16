using System.ComponentModel;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터
    public class FriendRequestListReq
    {
        public string MyToken { get; set; }
    }
    // 응답 데이터
    public class FriendRequestElement
    {
        public long RequestId { get; set; }
        public string FromUserName { get; set; }
        public string ToUserName { get; set; }
    }
    public class FriendRequestListRes : ErrorCodeDTO
    {
        public List<FriendRequestElement> FriendRequestList { get; set; }
    }
}