using Microsoft.AspNetCore.Identity;

namespace BookFlix.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
