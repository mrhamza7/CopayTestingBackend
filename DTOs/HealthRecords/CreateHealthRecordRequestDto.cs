using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.HealthRecords
{
    public class CreateHealthRecordRequestDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public Guid? AppointmentId { get; set; }
        
        public Guid? ClinicStaffId { get; set; }
        
        [Required]
        public string RecordType { get; set; } = string.Empty; // Medical Note, Test Result, Prescription, etc.
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public string FileUrl { get; set; } = string.Empty;
        
        public bool IsSharedWithPatient { get; set; } = false;
    }
}