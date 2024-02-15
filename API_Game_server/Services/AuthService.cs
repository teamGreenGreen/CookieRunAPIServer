namespace API_Game_Server.Services;

public class AuthService
{
    string authServerAddress;

    public AuthService(IConfiguration configuration)
    {
        authServerAddress = configuration.GetSection("AuthServer").Value + "/VerifyToken"; 
    }

    public async Task<EErrorCode> VerifyTokenToAuthServer(Int64 uid, string authToken)
    {
        HttpClient client = new();

        // Post 요청을 보내고, Http 응답의 상태를 반환받음
        HttpResponseMessage response = await client.PostAsJsonAsync(authServerAddress, new { AuthToken = authToken, Uid = uid });
        if(response is null || !ValidateResponse(response))
        {
            return EErrorCode.Auth_Fail_InvalidResponse;
        }

        // Http 요청에 대한 응답이 성공적으로 수신되었으면
        // Json 데이터를 읽어옴
        var authResult = await response.Content.ReadFromJsonAsync<EErrorCode>();
        // TODO : 에러 코드 검사하는 코드 추가
        // 다른 api 서버에서 보낸 에러 코드는 여기서 알 수 없기 때문에
        // 모든 에러를 Auth_Fail_InvalidResponse 같은 포괄적인 에러 코드로 처리해야 함
        
    }

    public bool ValidateResponse(HttpResponseMessage response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return false;
        }        
        return true;
    }
}
