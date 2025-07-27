using CP1Testing.DTOs.Patients;
using CP1Testing.DTOs.Insurance;
using CP1Testing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound(new { message = "Patient not found" });
            }

            return Ok(patient);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPatientByUserId(Guid userId)
        {
            var patient = await _patientService.GetPatientByUserIdAsync(userId);

            if (patient == null)
            {
                return NotFound(new { message = "Patient not found" });
            }

            return Ok(patient);
        }

        [HttpGet("clinic/{clinicId}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetPatientsByClinic(Guid clinicId)
        {
            var patients = await _patientService.GetPatientsByClinicAsync(clinicId);
            return Ok(patients);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.CreatePatientAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetPatientById), new { id = result.Patient.Id }, result.Patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.UpdatePatientAsync(id, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Patient);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var result = await _patientService.DeletePatientAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Patient not found" });
            }

            return NoContent();
        }

        // Insurance related endpoints
        [HttpGet("{patientId}/insurances")]
        public async Task<IActionResult> GetPatientInsurances(Guid patientId)
        {
            var insurances = await _patientService.GetPatientInsurancesAsync(patientId);
            return Ok(insurances);
        }

        [HttpPost("{patientId}/insurances")]
        public async Task<IActionResult> AddPatientInsurance(Guid patientId, [FromBody] CreateInsuranceRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.AddPatientInsuranceAsync(patientId, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Insurance);
        }

        [HttpPut("{patientId}/insurances/{insuranceId}")]
        public async Task<IActionResult> UpdatePatientInsurance(Guid patientId, Guid insuranceId, [FromBody] UpdateInsuranceRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.UpdatePatientInsuranceAsync(patientId, insuranceId, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Insurance);
        }

        [HttpDelete("{patientId}/insurances/{insuranceId}")]
        public async Task<IActionResult> DeletePatientInsurance(Guid patientId, Guid insuranceId)
        {
            var result = await _patientService.DeletePatientInsuranceAsync(patientId, insuranceId);

            if (!result)
            {
                return NotFound(new { message = "Insurance not found" });
            }

            return NoContent();
        }

        // Appointments related endpoints
        [HttpGet("{patientId}/appointments")]
        public async Task<IActionResult> GetPatientAppointments(Guid patientId)
        {
            var appointments = await _patientService.GetPatientAppointmentsAsync(patientId);
            return Ok(appointments);
        }

        // Health records related endpoints
        [HttpGet("{patientId}/health-records")]
        public async Task<IActionResult> GetPatientHealthRecords(Guid patientId)
        {
            var healthRecords = await _patientService.GetPatientHealthRecordsAsync(patientId);
            return Ok(healthRecords);
        }

        // Copayments related endpoints
        [HttpGet("{patientId}/copayments")]
        public async Task<IActionResult> GetPatientCopayments(Guid patientId)
        {
            var copayments = await _patientService.GetPatientCopaymentsAsync(patientId);
            return Ok(copayments);
        }
    }
}