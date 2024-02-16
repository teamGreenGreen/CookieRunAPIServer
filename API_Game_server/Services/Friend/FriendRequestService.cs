using API_Game_Server.Repository;
using API_Game_Server.Model.DTO;
using API_Game_Server.Model.DAO;
using System;

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
            // 토큰 유효성 검사
            string myUid = await validationService.GetUid(Token);
            // 유효하지 않은 토큰이면
            if(myUid == "")
            {
                return EErrorCode.InvalidToken;
            }

            // 가져온 uid 이용해서 클라이언트의 user_name 조회
            string uidKey = string.Format("user_info:uid:{0}", myUid);
            string[] arrUidValues = {"user_name"};
            string[] arrMyName = await redisDB.GetHash(uidKey, arrUidValues); // 받아올 칼럼명을 프로퍼티명으로 전달
            string myName = arrMyName[0];

            // 자기 자신의 이름인지 확인
            if (myName == ToUserName) // 보내는 username과 받는 username이 동일하면 자기가 자기한테 친구 신청
            {
                return EErrorCode.FriendReqFailSelfRequest;
            }

            // ToUserName의 유저가 존재하는지 확인
            FriendInfo myFriendInfo = await gameDB.GetFriendInfo(ToUserName);
            if(myFriendInfo == null)
            {
                return EErrorCode.FriendReqFailTargetNotFound;
            }

            // 이미 친구인지 확인
            string friendshipKey = string.Format("friend_relationship:{0}",ToUserName);
            FriendShipInfo friendshipInfo = new FriendShipInfo();
            friendshipInfo.IsExist = await redisDB.GetSetIsMemberExist(friendshipKey, ToUserName);
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
            string myFriendCountKey = string.Format("friend_relationship:{0}",myName);
            FriendCountInfo myFriendCount = new FriendCountInfo();
            myFriendCount.FriendCount = await redisDB.SizeOfSet(myFriendCountKey);
            if(myFriendCount.FriendCount >= 5) // Test를 위해 최대 친구 수 5로 수정 -> 나중에 50으로 바꿀 예정
            {
                return EErrorCode.FriendReqFailMyFriendCountExceeded;
            }

            // 역방향 신청 존재하는지 확인
            ReverseFriendShipInfo reverseFriendShipInfo = await gameDB.GetReverseFriendShipInfo(myName, ToUserName);
            if(reverseFriendShipInfo != null)
            {
                await gameDB.InsertFriendShip(myName,ToUserName);
                await gameDB.InsertFriendShip(ToUserName,myName); // 양방향 친구관계 생성
                return EErrorCode.None;
            }

            // 신청이 완료된 경우
            await gameDB.InsertFriendRequest(myName, ToUserName); // DB에 작성
            return EErrorCode.None;
        }
    }
}