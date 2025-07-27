using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Clinics
{
    public class CreateClinicRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        public string State { get; set; } = string.Empty;
        
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string Website { get; set; } = string.Empty;
        
        [Required]
        public string TaxId { get; set; } = string.Empty;
    }
}