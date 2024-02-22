using API_Game_Server.Model.DAO;

namespace API_Game_Server.Model.DTO
{
    // 응답 데이터
    public class UserInfoRes : ErrorCodeDTO
    {
        public UserInfo UserInfo { get; set; }
    }
}
