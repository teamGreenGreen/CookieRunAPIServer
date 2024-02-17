using System.Security.Cryptography;
using System.Text;

namespace API_Game_Server;

public class Security
{
    public static string GenerateSessionId()
    {
        byte[] bytes = new byte[8];
        using (RandomNumberGenerator randGenerator = RandomNumberGenerator.Create())
        {
            randGenerator.GetBytes(bytes);
        }

        string hexString = Convert.ToHexString(bytes);

        return hexString;
    }
}
