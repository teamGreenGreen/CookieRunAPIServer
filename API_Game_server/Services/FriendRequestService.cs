using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;

namespace API_Game_Server.Services
{
    public class FriendRequestService
    {
        private readonly GameDB gameDB;
        private readonly RedisDB redisDB;
        private readonly ValidationService validationService;
        public FriendRequestService(GameDB _gameDB, RedisDB _redisDB, ValidationService _validationService)
        {
            gameDB = _gameDB;
            redisDB = _redisDB;
            validationService = _validationService;
        }
        public async Task<EErrorCode> FriendRequest(string Token, string ToUserName)
        {
            string myUid = await validationService.GetUid(Token);
            // 유효하지 않은 토큰이면
            if(myUid == "")
            {
                return EErrorCode.InvalidToken;
            }

            string[] userName = {"userName"};
            string[] arrMyName = await redisDB.GetHash(myUid, userName); // 받아올 칼럼명을 프로퍼티명으로 전달
            string myName = arrMyName[0];

            // 자기 자신의 이름인지 확인
            if (myName == ToUserName) // 보내는 username과 받는 username이 동일하면 자기가 자기한테 친구 신청
            {
                return EErrorCode.FriendReqFailSelfRequest;
            }

            return EErrorCode.None;

            // ToUserName의 유저가 존재하는지 확인
            FriendInfo myFriendInfo = await gameDB.GetFriendInfo(ToUserName);
            if(myFriendInfo == null)
            {
                return EErrorCode.FriendReqFailTargetNotFound;
            }

            // 이미 친구인지 확인
            FriendShipInfo friendshipInfo = await gameDB.GetFriendShipInfo(myName, ToUserName);
            if(friendshipInfo != null)
            {
                return EErrorCode.FriendReqFailAlreadyFriend;
            }

            // 동일한 요청 존재하는지 확인
            FriendRequestInfo friendRequestInfo = await gameDB.GetFriendRequestInfo(myName, ToUserName);
            if(friendRequestInfo != null)
            {
                return EErrorCode.FriendReqFailAlreadyReqExist;
            }

            // 최대 친구 수 초과 확인
            FriendCountInfo myFriendCount = await gameDB.GetMyFriendCountInfo(myName);
            if(myFriendCount.FriendCount >= 50)
            {
                return EErrorCode.FriendReqFailMyFriendCountExceeded;
            }

            // // 역방향 신청 존재하는지 확인
            

            // // 신청이 완료된 경우
            // await gameDB.InsertFriendRequest(req.FromUserName, req.ToUserName);
            // return Ok(new {Message = "친구 신청이 성공적으로 처리되었습니다."});
        }
    }
}