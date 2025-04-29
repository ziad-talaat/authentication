using Authorization_Refreshtoken.DTO;
using Authorization_Refreshtoken.Models;

namespace Authorization_Refreshtoken.Service
{
    public interface IJWTService
    {
        AuthResponse CreateToken(AppUser user);
    }
}
