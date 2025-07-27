using CP1Testing.DTOs.Users;
using CP1Testing.Entities;
using CP1Testing.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CP1Testing.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userRepository.GetUserRolesAsync(user.Id);
                userDtos.Add(MapToUserDto(user, roles));
            }

            return userDtos;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);
            if (user == null) return null;

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            return MapToUserDto(user, roles);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            return MapToUserDto(user, roles);
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userRepository.GetUserRolesAsync(user.Id);
                userDtos.Add(MapToUserDto(user, roles));
            }

            return userDtos;
        }

        public async Task<(bool Success, string Message, UserDto? User)> CreateUserAsync(CreateUserRequestDto model)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Email is already registered", null);
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

            // Add roles
            if (model.Roles != null && model.Roles.Any())
            {
                foreach (var role in model.Roles)
                {
                    await _userRepository.AddToRoleAsync(user.Id, role);
                }
            }
            else
            {
                // Add default role if none specified
                await _userRepository.AddToRoleAsync(user.Id, "User");
            }

            // Get updated roles
            var roles = await _userRepository.GetUserRolesAsync(user.Id);

            return (true, "User created successfully", MapToUserDto(user, roles));
        }

        public async Task<(bool Success, string Message, UserDto? User)> UpdateUserAsync(Guid id, UpdateUserRequestDto model)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, "User not found", null);
            }

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return (false, "Email is already registered", null);
                }

                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
            }

            // Update user properties
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (model.IsActive.HasValue)
                user.IsActive = model.IsActive.Value;

            // Update roles if provided
            if (model.Roles != null && model.Roles.Any())
            {
                // Get current roles
                var currentRoles = await _userRepository.GetUserRolesAsync(user.Id);

                // Remove roles that are not in the new list
                foreach (var role in currentRoles)
                {
                    if (!model.Roles.Contains(role))
                    {
                        await _userRepository.RemoveFromRoleAsync(user.Id, role);
                    }
                }

                // Add roles that are not in the current list
                foreach (var role in model.Roles)
                {
                    if (!currentRoles.Contains(role))
                    {
                        await _userRepository.AddToRoleAsync(user.Id, role);
                    }
                }
            }

            // Save changes
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Get updated roles
            var updatedRoles = await _userRepository.GetUserRolesAsync(user.Id);

            return (true, "User updated successfully", MapToUserDto(user, updatedRoles));
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.SoftDeleteAsync(id);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null) return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            await _userRepository.UpdateAsync(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role)
        {
            return await _userRepository.IsInRoleAsync(userId, role);
        }

        public async Task<bool> AddToRoleAsync(Guid userId, string role)
        {
            return await _userRepository.AddToRoleAsync(userId, role);
        }

        public async Task<bool> RemoveFromRoleAsync(Guid userId, string role)
        {
            return await _userRepository.RemoveFromRoleAsync(userId, role);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }

        private UserDto MapToUserDto(User user, IEnumerable<string> roles)
        {
            return new UserDto
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
        }
    }
}