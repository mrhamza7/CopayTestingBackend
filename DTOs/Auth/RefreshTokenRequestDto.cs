using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}