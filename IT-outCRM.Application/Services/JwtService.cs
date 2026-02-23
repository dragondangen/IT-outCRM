using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IT_outCRM.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse JWT token");
                return null;
            }

            return null;
        }
    }
}

