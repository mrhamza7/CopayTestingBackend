using CP1Testing.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP1Testing.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<User?> FindByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken != null && u.RefreshToken == refreshToken);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
        }

        public async Task<User?> GetByIdWithRolesAsync(Guid id)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            var normalizedRoleName = roleName.ToUpper();
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.NormalizedName == normalizedRoleName))
                .ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string roleName)
        {
            var normalizedRoleName = roleName.ToUpper();
            var user = await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user != null && user.UserRoles.Any(ur => ur.Role.NormalizedName == normalizedRoleName);
        }

        public async Task<bool> AddToRoleAsync(Guid userId, string roleName)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper());
            if (role == null) return false;

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

            if (userRole != null) return true; // User already in role

            userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id
            };

            await _context.UserRoles.AddAsync(userRole);
            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveFromRoleAsync(Guid userId, string roleName)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpper());
            if (role == null) return false;

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);

            if (userRole == null) return true; // User not in role

            _context.UserRoles.Remove(userRole);
            return await SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Enumerable.Empty<string>();

            return user.UserRoles.Select(ur => ur.Role.Name).ToList();
        }
    }
}