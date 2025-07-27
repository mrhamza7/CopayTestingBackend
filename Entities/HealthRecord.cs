using System;

namespace CP1Testing.Entities
{
    public class HealthRecord : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? ClinicStaffId { get; set; }
        public string RecordType { get; set; } = string.Empty; // Medical Note, Test Result, Prescription, etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public bool IsSharedWithPatient { get; set; } = false;
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Appointment? Appointment { get; set; }
        public virtual ClinicStaff? ClinicStaff { get; set; }
    }
}