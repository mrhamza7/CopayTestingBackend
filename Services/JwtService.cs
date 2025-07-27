using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CP1Testing.Services
{
    public class JwtService : IJwtService
    {        
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Make sure we have a valid secret key
            if (string.IsNullOrEmpty(_jwtSettings.Secret))
            {
                throw new ArgumentException("JWT Secret key is not configured properly. Please check your configuration.");
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetAccessTokenExpirationTime()
        {
            return DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
        }

        public DateTime GetRefreshTokenExpirationTime()
        {
            return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        }

        public bool ValidateAccessToken(string token, out Guid userId, out string email, out IEnumerable<string> roles)
        {
            userId = Guid.Empty;
            email = string.Empty;
            roles = new List<string>();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Extract claims
                var userIdClaim = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                var emailClaim = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                var roleClaims = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

                userId = Guid.Parse(userIdClaim);
                email = emailClaim;
                roles = roleClaims;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateRefreshToken(string token)
        {
            // Refresh tokens are validated against the stored tokens in the database
            // This method is a placeholder as the actual validation happens in the auth service
            // where we check if the token exists in the database and is not expired or revoked
            return !string.IsNullOrEmpty(token) && token.Length >= 64;
        }
    }

    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}