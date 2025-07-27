using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Users
{
    public class UpdateUserRequestDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public List<string>? Roles { get; set; }
        
        public bool? IsActive { get; set; }
    }
}