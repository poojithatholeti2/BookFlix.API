using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookFlix.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BookFlix.API.Repositories
{
    public class SQLTokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public SQLTokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            //create claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //get key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_Key")));

            //generate credentials using the key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //generate token using claims, credentials
            var token = new JwtSecurityToken(
                Environment.GetEnvironmentVariable("JWT_Issuer"),
                Environment.GetEnvironmentVariable("JWT_Audience"),
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);


            //return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
