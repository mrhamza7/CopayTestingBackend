using CP1Testing.Entities;

namespace CP1Testing.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdWithRolesAsync(Guid id);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> IsInRoleAsync(Guid userId, string roleName);
        Task<bool> AddToRoleAsync(Guid userId, string roleName);
        Task<bool> RemoveFromRoleAsync(Guid userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
        Task<User?> FindByRefreshTokenAsync(string refreshToken);
    }
}