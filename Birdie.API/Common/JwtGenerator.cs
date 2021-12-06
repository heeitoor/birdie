using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Birdie.API.Common
{
    public interface IJwtGenerator
    {
        TokenResponse GetToken(int userId, string name);
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
    }

    public class JwtGenerator : IJwtGenerator
    {
        private readonly IConfiguration configuration;

        public JwtGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public TokenResponse GetToken(int userId, string name)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JwtKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, name)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(token)
            };
        }
    }
}
