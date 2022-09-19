using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace TelloWebApi.Helper
{
    public class Helper
    {

        public static void DeleteImage(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

        }

        public enum UserRoles
        {

            Admin,
            Member,
            SuperAdmin
        }


        public static string DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (token == null)
                return null;
            var decoded = handler.ReadJwtToken(token.Replace("Bearer ", ""));

            return decoded.Claims.First(claim => claim.Type == "nameid").Value;
        }
    }
}
