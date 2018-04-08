using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductsApi.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:7575/"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 1 * 60 * 6; // время жизни токена - 6 часов
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey( Encoding.ASCII.GetBytes( KEY ) );
        }
    }
}
