using CP1Testing.DTOs.HealthRecords;
using CP1Testing.Entities;
using CP1Testing.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CP1Testing.Services
{
    public class HealthRecordService : IHealthRecordService
    {
        private readonly IRepository<HealthRecord> _healthRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IRepository<ClinicStaff> _clinicStaffRepository;
        private readonly IRepository<Appointment> _appointmentRepository;

        public HealthRecordService(
            IRepository<HealthRecord> healthRecordRepository,
            IPatientRepository patientRepository,
            IRepository<ClinicStaff> clinicStaffRepository,
            IRepository<Appointment> appointmentRepository)
        {
            _healthRecordRepository = healthRecordRepository;
            _patientRepository = patientRepository;
            _clinicStaffRepository = clinicStaffRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<HealthRecordDto>> GetAllHealthRecordsAsync()
        {
            var healthRecords = await _healthRecordRepository.GetAllAsync();
            var healthRecordDtos = new List<HealthRecordDto>();

            foreach (var record in healthRecords)
            {
                healthRecordDtos.Add(await MapToHealthRecordDtoAsync(record));
            }

            return healthRecordDtos;
        }

        public async Task<HealthRecordDto?> GetHealthRecordByIdAsync(Guid id)
        {
            var healthRecord = await _healthRecordRepository.GetByIdAsync(id);
            if (healthRecord == null) return null;

            return await MapToHealthRecordDtoAsync(healthRecord);
        }

        public async Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByPatientAsync(Guid patientId)
        {
            var healthRecords = await _healthRecordRepository.FindAsync(hr => hr.PatientId == patientId);
            var healthRecordDtos = new List<HealthRecordDto>();

            foreach (var record in healthRecords)
            {
                healthRecordDtos.Add(await MapToHealthRecordDtoAsync(record));
            }

            return healthRecordDtos;
        }

        public async Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByAppointmentAsync(Guid appointmentId)
        {
            var healthRecords = await _healthRecordRepository.FindAsync(hr => hr.AppointmentId == appointmentId);
            var healthRecordDtos = new List<HealthRecordDto>();

            foreach (var record in healthRecords)
            {
                healthRecordDtos.Add(await MapToHealthRecordDtoAsync(record));
            }

            return healthRecordDtos;
        }

        public async Task<IEnumerable<HealthRecordDto>> GetHealthRecordsByClinicStaffAsync(Guid clinicStaffId)
        {
            var healthRecords = await _healthRecordRepository.FindAsync(hr => hr.ClinicStaffId == clinicStaffId);
            var healthRecordDtos = new List<HealthRecordDto>();

            foreach (var record in healthRecords)
            {
                healthRecordDtos.Add(await MapToHealthRecordDtoAsync(record));
            }

            return healthRecordDtos;
        }

        public async Task<(bool Success, string Message, HealthRecordDto? HealthRecord)> CreateHealthRecordAsync(CreateHealthRecordRequestDto model)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(model.PatientId);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            // Validate appointment if provided
            if (model.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(model.AppointmentId.Value);
                if (appointment == null)
                {
                    return (false, "Appointment not found", null);
                }

                // Ensure appointment belongs to the patient
                if (appointment.PatientId != model.PatientId)
                {
                    return (false, "Appointment does not belong to the specified patient", null);
                }
            }

            // Validate clinic staff if provided
            if (model.ClinicStaffId.HasValue)
            {
                var clinicStaff = await _clinicStaffRepository.GetByIdAsync(model.ClinicStaffId.Value);
                if (clinicStaff == null)
                {
                    return (false, "Clinic staff not found", null);
                }
            }

            // Create health record
            var healthRecord = new HealthRecord
            {
                PatientId = model.PatientId,
                AppointmentId = model.AppointmentId,
                ClinicStaffId = model.ClinicStaffId,
                RecordType = model.RecordType,
                Title = model.Title,
                Description = model.Description,
                FileUrl = model.FileUrl,
                IsSharedWithPatient = model.IsSharedWithPatient
            };

            await _healthRecordRepository.AddAsync(healthRecord);

            return (true, "Health record created successfully", await MapToHealthRecordDtoAsync(healthRecord));
        }

        public async Task<(bool Success, string Message, HealthRecordDto? HealthRecord)> UpdateHealthRecordAsync(Guid id, UpdateHealthRecordRequestDto model)
        {
            var healthRecord = await _healthRecordRepository.GetByIdAsync(id);
            if (healthRecord == null)
            {
                return (false, "Health record not found", null);
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(model.RecordType))
            {
                healthRecord.RecordType = model.RecordType;
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                healthRecord.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                healthRecord.Description = model.Description;
            }

            if (!string.IsNullOrEmpty(model.FileUrl))
            {
                healthRecord.FileUrl = model.FileUrl;
            }

            if (model.IsSharedWithPatient.HasValue)
            {
                healthRecord.IsSharedWithPatient = model.IsSharedWithPatient.Value;
            }

            await _healthRecordRepository.UpdateAsync(healthRecord);

            return (true, "Health record updated successfully", await MapToHealthRecordDtoAsync(healthRecord));
        }

        public async Task<bool> DeleteHealthRecordAsync(Guid id)
        {
            return await _healthRecordRepository.SoftDeleteAsync(id);
        }

        public async Task<(bool Success, string Message)> ShareHealthRecordWithPatientAsync(Guid id, bool isShared)
        {
            var healthRecord = await _healthRecordRepository.GetByIdAsync(id);
            if (healthRecord == null)
            {
                return (false, "Health record not found");
            }

            healthRecord.IsSharedWithPatient = isShared;
            await _healthRecordRepository.UpdateAsync(healthRecord);

            string action = isShared ? "shared with" : "hidden from";
            return (true, $"Health record successfully {action} patient");
        }

        #region Helper Methods

        private async Task<HealthRecordDto> MapToHealthRecordDtoAsync(HealthRecord record)
        {
            string patientName = string.Empty;
            string clinicStaffName = string.Empty;

            // Get patient name
            var patient = await _patientRepository.GetByIdWithDetailsAsync(record.PatientId);
            if (patient?.User != null)
            {
                patientName = $"{patient.User.FirstName} {patient.User.LastName}";
            }

            // Get clinic staff name
            if (record.ClinicStaffId.HasValue)
            {
                var clinicStaff = await _clinicStaffRepository.GetByIdAsync(record.ClinicStaffId.Value);
                if (clinicStaff != null)
                {
                    // We need to get the user associated with the clinic staff
                    // This would typically be done with a join in the repository, but we'll use a simple approach here
                    var contextField = typeof(Repository<ClinicStaff>)
                        .GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (contextField != null)
                    {
                        var context = contextField.GetValue(_clinicStaffRepository) as ApplicationDbContext;
                        if (context != null)
                        {
                            var user = await context.Users.FindAsync(clinicStaff.UserId);
                            if (user != null)
                            {
                                clinicStaffName = $"{user.FirstName} {user.LastName}";
                            }
                        }
                    }
                }
            }

            // Get appointment date if applicable
            DateTime? appointmentDate = null;
            if (record.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(record.AppointmentId.Value);
                appointmentDate = appointment?.AppointmentDate;
            }

            return new HealthRecordDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = patientName,
                AppointmentId = record.AppointmentId,
                AppointmentDate = appointmentDate,
                ClinicStaffId = record.ClinicStaffId,
                ClinicStaffName = clinicStaffName,
                RecordType = record.RecordType,
                Title = record.Title,
                Description = record.Description,
                FileUrl = record.FileUrl,
                IsSharedWithPatient = record.IsSharedWithPatient,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }

        #endregion
    }
}