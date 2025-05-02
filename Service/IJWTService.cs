using Authorization_Refreshtoken.DTO;
using Authorization_Refreshtoken.Models;
using System.Security.Claims;

namespace Authorization_Refreshtoken.Service
{
    public interface IJWTService
    {
        AuthResponse CreateToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrinciplesFromJWTToken(string token);

    }
}
