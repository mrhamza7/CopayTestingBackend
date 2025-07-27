using CP1Testing.DTOs.Clinics;
using CP1Testing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllClinics()
        {
            var clinics = await _clinicService.GetAllClinicsAsync();
            return Ok(clinics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinicById(Guid id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);

            if (clinic == null)
            {
                return NotFound(new { message = "Clinic not found" });
            }

            return Ok(clinic);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clinicService.CreateClinicAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetClinicById), new { id = result.Clinic.Id }, result.Clinic);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clinicService.UpdateClinicAsync(id, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Clinic);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteClinic(Guid id)
        {
            var result = await _clinicService.DeleteClinicAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Clinic not found" });
            }

            return NoContent();
        }

        [HttpPost("assign-admin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignClinicAdmin([FromBody] AssignClinicAdminRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _clinicService.AssignClinicAdminAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpGet("{id}/admins")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetClinicAdmins(Guid id)
        {
            var admins = await _clinicService.GetClinicAdminsAsync(id);

            if (admins == null)
            {
                return NotFound(new { message = "Clinic not found" });
            }

            return Ok(admins);
        }

        [HttpGet("{id}/staff")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetClinicStaff(Guid id)
        {
            var clinic = await _clinicService.GetClinicByIdAsync(id);
            if (clinic == null)
            {
                return NotFound(new { message = "Clinic not found" });
            }

            var staff = await _clinicService.GetClinicStaffAsync(id);

            return Ok(staff);
        }
    }
}