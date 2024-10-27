using FinalProject3.Models;

namespace FinalProject3.Auth
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
