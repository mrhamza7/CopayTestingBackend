using System;

namespace CP1Testing.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? ClinicStaffId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty; // Scheduled, Confirmed, Completed, Cancelled, No-Show
        public string Type { get; set; } = string.Empty; // Regular, Follow-up, Emergency, etc.
        public string Notes { get; set; } = string.Empty;
        public string CancellationReason { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Clinic Clinic { get; set; } = null!;
        public virtual ClinicStaff? ClinicStaff { get; set; }
        public virtual Copayment? Copayment { get; set; }
    }
}