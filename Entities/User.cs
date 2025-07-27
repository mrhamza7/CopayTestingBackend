using System;

namespace CP1Testing.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public bool EmailConfirmed { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<ClinicAdmin> ClinicAdmins { get; set; } = new List<ClinicAdmin>();
        
        // Helper properties
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}