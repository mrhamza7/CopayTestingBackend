using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Clinics
{
    public class UpdateClinicRequestDto
    {
        public string? Name { get; set; }
        
        public string? Address { get; set; }
        
        public string? City { get; set; }
        
        public string? State { get; set; }
        
        public string? ZipCode { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Website { get; set; }
        
        public string? TaxId { get; set; }
        
        public bool? IsActive { get; set; }
    }
}