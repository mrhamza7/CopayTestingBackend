using CP1Testing.DTOs.Appointments;
using CP1Testing.DTOs.Copayments;
using CP1Testing.DTOs.HealthRecords;
using CP1Testing.DTOs.Insurance;
using CP1Testing.DTOs.Patients;

namespace CP1Testing.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(Guid id);
        Task<PatientDto?> GetPatientByUserIdAsync(Guid userId);
        Task<IEnumerable<PatientDto>> GetPatientsByClinicAsync(Guid clinicId);
        Task<(bool Success, string Message, PatientDto? Patient)> CreatePatientAsync(CreatePatientRequestDto model);
        Task<(bool Success, string Message, PatientDto? Patient)> UpdatePatientAsync(Guid id, UpdatePatientRequestDto model);
        Task<bool> DeletePatientAsync(Guid id);
        
        // Insurance related methods
        Task<IEnumerable<InsuranceDto>> GetPatientInsurancesAsync(Guid patientId);
        Task<InsuranceDto?> GetInsuranceByIdAsync(Guid id);
        Task<(bool Success, string Message, InsuranceDto? Insurance)> CreateInsuranceAsync(CreateInsuranceRequestDto model);
        Task<(bool Success, string Message, InsuranceDto? Insurance)> AddPatientInsuranceAsync(Guid patientId, CreateInsuranceRequestDto model);
        Task<(bool Success, string Message, InsuranceDto? Insurance)> UpdateInsuranceAsync(Guid id, UpdateInsuranceRequestDto model);
        Task<(bool Success, string Message, InsuranceDto? Insurance)> UpdatePatientInsuranceAsync(Guid patientId, Guid insuranceId, UpdateInsuranceRequestDto model);
        Task<bool> DeleteInsuranceAsync(Guid id);
        Task<bool> DeletePatientInsuranceAsync(Guid patientId, Guid insuranceId);
        
        // Appointment related methods
        Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId);
        
        // Health record related methods
        Task<IEnumerable<HealthRecordDto>> GetPatientHealthRecordsAsync(Guid patientId);
        
        // Copayment related methods
        Task<IEnumerable<CopaymentDto>> GetPatientCopaymentsAsync(Guid patientId);
    }
}