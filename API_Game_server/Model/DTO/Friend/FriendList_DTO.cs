using System.ComponentModel;
using API_Game_Server.Model.DAO;

namespace API_Game_Server.Model.DTO
{
    //  요청 데이터

    // 응답 데이터
    public class FriendListRes : ErrorCodeDTO
    {
        public IEnumerable<FriendElement> FriendList { get; set; } // DAO의 친구 요소로 이루어진 List를 반환
    }
}