using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.HealthRecords
{
    public class UpdateHealthRecordRequestDto
    {
        public string? RecordType { get; set; } // Medical Note, Test Result, Prescription, etc.
        
        public string? Title { get; set; }
        
        public string? Description { get; set; }
        
        public string? FileUrl { get; set; }
        
        public bool? IsSharedWithPatient { get; set; }
    }
}