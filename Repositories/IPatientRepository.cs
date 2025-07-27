using CP1Testing.Entities;

namespace CP1Testing.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<Patient>> GetPatientsByClinicAsync(Guid clinicId);
        Task<IEnumerable<Insurance>> GetPatientInsurancesAsync(Guid patientId);
        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(Guid patientId);
        Task<IEnumerable<HealthRecord>> GetPatientHealthRecordsAsync(Guid patientId);
        Task<IEnumerable<Copayment>> GetPatientCopaymentsAsync(Guid patientId);
    }
}