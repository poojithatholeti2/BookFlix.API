using Microsoft.AspNetCore.Identity;

namespace BookFlix.API.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
