using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Clinics
{
    public class AssignClinicAdminRequestDto
    {
        [Required]
        public Guid ClinicId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
    }
}