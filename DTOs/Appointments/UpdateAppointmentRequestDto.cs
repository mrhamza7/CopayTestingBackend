using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Appointments
{
    public class UpdateAppointmentRequestDto
    {
        public Guid? ClinicStaffId { get; set; }
        
        public DateTime? AppointmentDate { get; set; }
        
        public TimeSpan? StartTime { get; set; }
        
        public TimeSpan? EndTime { get; set; }
        
        public string? Status { get; set; } // Scheduled, Confirmed, Completed, Cancelled, No-Show
        
        public string? Type { get; set; } // Regular, Follow-up, Emergency, etc.
        
        public string? Notes { get; set; }
        
        public string? CancellationReason { get; set; }
    }
}