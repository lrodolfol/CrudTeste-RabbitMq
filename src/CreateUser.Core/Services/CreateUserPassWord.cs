using CreateUser.Core.Entities;
using System.Security.Cryptography;
using System.Text;

namespace CreateUser.Core.Services;
public class CreateUserPassWord
{
    public static string CreatePassword(User user)
    {
        var unionValues = string.Join('~', user.Name, user.Email, user.Document, user.Phone);
        byte[] rawDataBytes = Encoding.UTF8.GetBytes(unionValues);

        using (var sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(rawDataBytes);
            string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedPassword.Substring(0, 7);
        }
    }
}
