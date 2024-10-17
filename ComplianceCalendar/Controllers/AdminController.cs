using ComplianceCalendar.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComplianceCalendar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository repository, ILogger<AdminController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetAdmins()
        {
            _logger.LogInformation("GetAdmins endpoint called");

            var admins = await _repository.GetAdminsAsync();

            if (admins == null || !admins.Any())
            {
                _logger.LogWarning("No admins found");
                return NotFound("No admins found.");
            }

            _logger.LogInformation("Admins retrieved successfully");
            return Ok(admins);
        }

        [HttpGet("GetDepartmentNamesByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetDepartmentNamesByEmployeeId(int employeeId)
        {
            _logger.LogInformation($"GetDepartmentNamesByEmployeeId endpoint called with employeeId: {employeeId}");

            var departments = await _repository.GetDepartmentNamesByAdminIdAsync(employeeId);

            if (departments == null || !departments.Any())
            {
                _logger.LogWarning($"No departments found for employeeId: {employeeId}");
                return NotFound($"No departments found for employeeId: {employeeId}");
            }

            _logger.LogInformation($"Departments retrieved successfully for employeeId: {employeeId}");
            return Ok(departments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            _logger.LogInformation($"DeleteAdmin endpoint called with id: {id}");

            var admin = await _repository.DeleteAdminAsync(id);

            if (admin == null)
            {
                _logger.LogWarning($"Admin with id: {id} not found");
                return NotFound("Employee not found.");
            }

            _logger.LogInformation($"Admin with id: {id} deleted successfully");
            return NoContent();
        }

        [HttpPost("AddAdmin")]
        public async Task<ActionResult<AdminDTO>> AddAdmin(AddAdminDTO model)
        {
            _logger.LogInformation("AddAdmin endpoint called");

            try
            {
                var admin = await _repository.AddAdminAsync(model);
                _logger.LogInformation($"Admin added successfully");
                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add admin");
                return StatusCode(500, $"Failed to add admin: {ex.Message}. Inner Exception: {ex.InnerException?.Message}");
            }
        }

        [HttpPut("{id}")]
        public IResult UpdateAdminStatus(int id)
        {
            _logger.LogInformation($"UpdateAdminStatus endpoint called with id: {id}");
            return _repository.UpdateAdminStatusAsync(id);
        }

        [HttpGet("GetEmployeesByDepartmentId/{departmentId}")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetEmployeesByDepartmentId(int departmentId)
        {
            _logger.LogInformation($"GetEmployeesByDepartmentId endpoint called with departmentId: {departmentId}");

            var employees = await _repository.GetEmployeesByDepartmentIdAsync(departmentId);

            if (employees == null || !employees.Any())
            {
                _logger.LogWarning($"No employees found for departmentId: {departmentId}");
                return NotFound("No employees found for this department.");
            }

            _logger.LogInformation($"Employees retrieved successfully for departmentId: {departmentId}");
            return Ok(employees);
        }

        [HttpGet("GetEmployeesByDepartmentName/{departmentName}")]
        public async Task<ActionResult<IEnumerable<AdminDTO>>> GetEmployeesByDepartmentName(string departmentName)
        {
            _logger.LogInformation($"GetEmployeesByDepartmentId endpoint called with departmentId: {departmentName}");

            var employees = await _repository.GetEmployeesByDepartmentNameAsync(departmentName);

            if (employees == null || !employees.Any())
            {
                _logger.LogWarning($"No employees found for departmentId: {departmentName}");
                return NotFound("No employees found for this department.");
            }

            _logger.LogInformation($"Employees retrieved successfully for departmentId: {departmentName}");
            return Ok(employees);
        }
    }
}
