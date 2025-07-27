namespace CP1Testing.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpirationTime();
        DateTime GetRefreshTokenExpirationTime();
        bool ValidateAccessToken(string token, out Guid userId, out string email, out IEnumerable<string> roles);
        bool ValidateRefreshToken(string token);
    }
}