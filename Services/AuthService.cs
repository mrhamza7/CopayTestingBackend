using CP1Testing.DTOs.Auth;
using CP1Testing.DTOs.Users;
using CP1Testing.Entities;
using CP1Testing.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CP1Testing.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> RegisterAsync(RegisterRequestDto model)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Email is already registered", null, null, null);
            }

            // Create new user
            var user = new User
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                IsActive = true
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            // Save user
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Add to Patient role by default
            await _userRepository.AddToRoleAsync(user.Id, "Patient");

            // Generate tokens
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _jwtService.GetRefreshTokenExpirationTime();
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsActive = user.IsActive,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = roles.ToList()
            };

            return (true, "Registration successful", userDto, accessToken, refreshToken);
        }

        public async Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> LoginAsync(LoginRequestDto model)
        {
            return await LoginAsync(model.Email, model.Password);
        }

        public async Task<(bool Success, string Message, UserDto? User, string? AccessToken, string? RefreshToken)> LoginAsync(string email, string password)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, "Invalid email or password", null, null, null);
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return (false, "Invalid email or password", null, null, null);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return (false, "Account is disabled", null, null, null);
            }

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;

            // Generate tokens
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _jwtService.GetRefreshTokenExpirationTime();
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Map to DTO
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsActive = user.IsActive,
                LastLoginDate = user.LastLoginDate,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = roles.ToList()
            };

            return (true, "Login successful", userDto, accessToken, refreshToken);
        }

        public async Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // Validate refresh token format
            if (!_jwtService.ValidateRefreshToken(refreshToken))
            {
                return (false, "Invalid refresh token", null, null);
            }

            // Find user by refresh token
            var users = await _userRepository.FindAsync(u => u.RefreshToken == refreshToken);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                return (false, "Invalid refresh token", null, null);
            }

            // Check if refresh token is expired
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return (false, "Refresh token expired", null, null);
            }

            // Generate new tokens
            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, roles);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Save new refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = _jwtService.GetRefreshTokenExpirationTime();
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Token refresh successful", accessToken, newRefreshToken);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.FindByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokeAllRefreshTokensAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            // Update password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Update password
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }
    }
}