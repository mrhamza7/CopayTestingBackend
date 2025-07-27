using CP1Testing.DTOs.HealthRecords;
using CP1Testing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HealthRecordController : ControllerBase
    {
        private readonly IHealthRecordService _healthRecordService;

        public HealthRecordController(IHealthRecordService healthRecordService)
        {
            _healthRecordService = healthRecordService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetAllHealthRecords()
        {
            var healthRecords = await _healthRecordService.GetAllHealthRecordsAsync();
            return Ok(healthRecords);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHealthRecordById(Guid id)
        {
            var healthRecord = await _healthRecordService.GetHealthRecordByIdAsync(id);

            if (healthRecord == null)
            {
                return NotFound(new { message = "Health record not found" });
            }

            return Ok(healthRecord);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetHealthRecordsByPatient(Guid patientId)
        {
            var healthRecords = await _healthRecordService.GetHealthRecordsByPatientAsync(patientId);
            return Ok(healthRecords);
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetHealthRecordsByAppointment(Guid appointmentId)
        {
            var healthRecords = await _healthRecordService.GetHealthRecordsByAppointmentAsync(appointmentId);
            return Ok(healthRecords);
        }

        [HttpGet("staff/{clinicStaffId}")]
        public async Task<IActionResult> GetHealthRecordsByClinicStaff(Guid clinicStaffId)
        {
            var healthRecords = await _healthRecordService.GetHealthRecordsByClinicStaffAsync(clinicStaffId);
            return Ok(healthRecords);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> CreateHealthRecord([FromBody] CreateHealthRecordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _healthRecordService.CreateHealthRecordAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetHealthRecordById), new { id = result.HealthRecord.Id }, result.HealthRecord);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> UpdateHealthRecord(Guid id, [FromBody] UpdateHealthRecordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _healthRecordService.UpdateHealthRecordAsync(id, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.HealthRecord);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> DeleteHealthRecord(Guid id)
        {
            var result = await _healthRecordService.DeleteHealthRecordAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Health record not found" });
            }

            return NoContent();
        }

        [HttpPost("{id}/share")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> ShareHealthRecordWithPatient(Guid id, [FromQuery] bool isShared = true)
        {
            var result = await _healthRecordService.ShareHealthRecordWithPatientAsync(id, isShared);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}