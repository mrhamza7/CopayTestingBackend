using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Patients
{
    public class UpdatePatientRequestDto
    {
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Gender { get; set; }
        
        public string? EmergencyContactName { get; set; }
        
        [Phone]
        public string? EmergencyContactPhone { get; set; }
        
        public string? EmergencyContactRelationship { get; set; }
    }
}