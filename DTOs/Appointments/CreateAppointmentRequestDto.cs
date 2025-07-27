using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Appointments
{
    public class CreateAppointmentRequestDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid ClinicId { get; set; }
        
        public Guid? ClinicStaffId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        [Required]
        public string Type { get; set; } = string.Empty; // Regular, Follow-up, Emergency, etc.
        
        public string Notes { get; set; } = string.Empty;
    }
}