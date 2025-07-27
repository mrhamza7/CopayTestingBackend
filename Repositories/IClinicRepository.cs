using CP1Testing.Entities;

namespace CP1Testing.Repositories
{
    public interface IClinicRepository : IRepository<Clinic>
    {
        Task<Clinic?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Clinic>> GetAllWithDetailsAsync();
        Task<IEnumerable<ClinicAdmin>> GetClinicAdminsAsync(Guid clinicId);
        Task<IEnumerable<ClinicStaff>> GetClinicStaffAsync(Guid clinicId);
        Task<bool> AddClinicAdminAsync(Guid clinicId, Guid userId);
        Task<bool> RemoveClinicAdminAsync(Guid clinicId, Guid userId);
    }
}