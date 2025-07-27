using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Patients
{
    public class CreatePatientRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        public Guid ClinicId { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        public string EmergencyContactName { get; set; } = string.Empty;
        
        [Phone]
        public string EmergencyContactPhone { get; set; } = string.Empty;
        
        public string EmergencyContactRelationship { get; set; } = string.Empty;
    }
}