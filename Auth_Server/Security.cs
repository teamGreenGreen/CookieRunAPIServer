using System.Security.Cryptography;
using System.Text;

namespace API_Game_Server;

public class Security
{
    const string ALLOWABLE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateHashingPassword(string saltValue, string password)
    {
        SHA256 sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + password));

        // 해쉬로 생성한 바이트의 자리수를 맞추기 위해서 2자리수인 16진수로 변환
        string hexString = Convert.ToHexString(hash);

        return hexString;
    }

    public static string GenerateSaltString()
    {
        byte[] bytes = new byte[32];
        using (RandomNumberGenerator randGenerator = RandomNumberGenerator.Create())
        {
            randGenerator.GetBytes(bytes);
        }

        string hexString = Convert.ToHexString(bytes);

        return hexString;
    }

    public static string GenerateAuthToken(string saltValue, Int64 uid)
    {
        SHA256 sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltValue + uid));

        string hexString = Convert.ToHexString(hash);

        return hexString;
    }
}
