using CP1Testing.DTOs.HealthRecords;

namespace CP1Testing.Services
{
    public interface IHealthRecordService
    {
        Task<IEnumerable<HealthRecordDto>> GetAllHealthRecordsAsync();
        Task<HealthRecordDto?> GetHealthRecordByIdAsync(Guid id);
        Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByPatientAsync(Guid patientId);
        Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByAppointmentAsync(Guid appointmentId);
        Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByClinicStaffAsync(Guid clinicStaffId);
        Task<(bool Success, string Message, HealthRecordDto? HealthRecord)> CreateHealthRecordAsync(CreateHealthRecordRequestDto model);
        Task<(bool Success, string Message, HealthRecordDto? HealthRecord)> UpdateHealthRecordAsync(Guid id, UpdateHealthRecordRequestDto model);
        Task<bool> DeleteHealthRecordAsync(Guid id);
        Task<(bool Success, string Message)> ShareHealthRecordWithPatientAsync(Guid id, bool isShared);
    }
}