using CP1Testing.DTOs.Clinics;
using CP1Testing.DTOs.Users;

namespace CP1Testing.Services
{
    public interface IClinicService
    {
        Task<IEnumerable<ClinicDto>> GetAllClinicsAsync();
        Task<ClinicDto?> GetClinicByIdAsync(Guid id);
        Task<(bool Success, string Message, ClinicDto? Clinic)> CreateClinicAsync(CreateClinicRequestDto model);
        Task<(bool Success, string Message, ClinicDto? Clinic)> UpdateClinicAsync(Guid id, UpdateClinicRequestDto model);
        Task<bool> DeleteClinicAsync(Guid id);
        Task<IEnumerable<UserDto>> GetClinicAdminsAsync(Guid clinicId);
        Task<IEnumerable<UserDto>> GetClinicStaffAsync(Guid clinicId);
        Task<(bool Success, string Message)> AssignClinicAdminAsync(AssignClinicAdminRequestDto model);
        Task<(bool Success, string Message)> RemoveClinicAdminAsync(Guid clinicId, Guid userId);
    }
}