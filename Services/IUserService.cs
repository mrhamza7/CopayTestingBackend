using CP1Testing.DTOs.Users;

namespace CP1Testing.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
        Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserRequestDto model);
        Task<(bool Success, string Message, UserDto? User)> UpdateUserAsync(Guid id, UpdateUserRequestDto model);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto model);
        Task<bool> IsInRoleAsync(Guid userId, string role);
        Task<bool> AddToRoleAsync(Guid userId, string role);
        Task<bool> RemoveFromRoleAsync(Guid userId, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
    }
}