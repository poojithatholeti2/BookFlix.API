using Microsoft.AspNetCore.Identity;

namespace BookFlix.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
