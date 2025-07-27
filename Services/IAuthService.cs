using CP1Testing.DTOs.Auth;
using CP1Testing.DTOs.Users;

namespace CP1Testing.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> RegisterAsync(RegisterRequestDto model);
        Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> LoginAsync(string email, string password);
        Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequestDto model);
        Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> RevokeAllRefreshTokensAsync(Guid userId);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(Guid userId, string newPassword);
    }
}