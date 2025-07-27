using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Users
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}