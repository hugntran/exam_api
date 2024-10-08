using DMAWS_T2305M_TranHung.Data;
using DMAWS_T2305M_TranHung.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAWS_T2305M_TranHung.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		readonly private T2305mApiContext _context;
		public EmployeesController(T2305mApiContext context)
		{
			_context = context;
		}

		[HttpGet("GetEmployees")]
		public async Task<ActionResult<IEnumerable<GetBasicEmployeeDTO>>> GetBasicEmployeeDTOs([FromQuery] EmployeeQueryParameters queryParameters)
		{
			try
			{
				// Build the query
				var employeesQuery = _context.Employees.AsQueryable();

				// Apply filters based on the query parameters
				if (!string.IsNullOrEmpty(queryParameters.EmployeeName))
				{
					employeesQuery = employeesQuery.Where(e => e.EmployeeName.Contains(queryParameters.EmployeeName));
				}

				if (queryParameters.EmployeeDOBFromDate.HasValue)
				{
					employeesQuery = employeesQuery.Where(e => e.EmployeeDOB >= queryParameters.EmployeeDOBFromDate.Value);
				}

				if (queryParameters.EmployeeDOBToDate.HasValue)
				{
					employeesQuery = employeesQuery.Where(e => e.EmployeeDOB <= queryParameters.EmployeeDOBToDate.Value);
				}

				// Execute the query and project the results into DTOs
				var employeeDTOs = await employeesQuery
					.Select(e => new GetBasicEmployeeDTO
					{
						EmployeeId = e.EmployeeId,
						EmployeeName = e.EmployeeName
					})
					.ToListAsync();

				return Ok(employeeDTOs);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}



		[HttpGet("GetDetailEmployee/{employeeId}")]
		public async Task<ActionResult<GetDetailEmployeeDTO>> GetDetailEmployeeDTO(int employeeId)
		{
			try
			{
				// Fetch the employee including related projects
				var employee = await _context.Employees
					.Include(e => e.ProjectEmployees)
						.ThenInclude(pe => pe.Projects) // Include the projects for the employee
					.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

				// Check if the employee exists
				if (employee == null)
				{
					return NotFound($"Employee with ID {employeeId} not found.");
				}

				// Map the employee to GetDetailEmployeeDTO
				var employeeDetails = new GetDetailEmployeeDTO
				{
					EmployeeId = employee.EmployeeId,
					EmployeeName = employee.EmployeeName,
					EmployeeDOB = employee.EmployeeDOB,
					EmployeeDepartment = employee.EmployeeDepartment,
					BasicProjectDTOs = employee.ProjectEmployees.Select(pe => new GetBasicProjectDTO
					{
						ProjectId = pe.ProjectId,
						ProjectName = pe.Projects.ProjectName,
						ProjectStartDate = pe.Projects.ProjectStartDate,
						ProjectEndDate = pe.Projects.ProjectEndDate
					}).ToList()
				};

				return Ok(employeeDetails);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}


	}
}
