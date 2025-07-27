using CP1Testing.DTOs.Appointments;
using CP1Testing.DTOs.Copayments;

namespace CP1Testing.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicAsync(Guid clinicId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicStaffAsync(Guid clinicStaffId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(Guid patientId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicAndDateAsync(Guid clinicId, DateTime date);
        Task<(bool Success, string Message, AppointmentDto? Appointment)> CreateAppointmentAsync(CreateAppointmentRequestDto model);
        Task<(bool Success, string Message, AppointmentDto? Appointment)> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto model);
        Task<(bool Success, string Message)> CancelAppointmentAsync(Guid id, string cancellationReason);
        Task<bool> DeleteAppointmentAsync(Guid id);
        
        // Copayment related methods
        Task<CopaymentDto?> GetAppointmentCopaymentAsync(Guid appointmentId);
        Task<(bool Success, string Message, CopaymentDto? Copayment)> CreateCopaymentAsync(CreateCopaymentRequestDto model);
        Task<(bool Success, string Message, CopaymentDto? Copayment)> UpdateCopaymentAsync(Guid id, UpdateCopaymentRequestDto model);
    }
}