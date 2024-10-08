using DMAWS_T2305M_TranHung.Data;
using DMAWS_T2305M_TranHung.Models;
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

		// GET: api/Employees
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
		{
			return await _context.Employees.ToListAsync();
		}

		// GET: api/Employees/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Employee>> GetEmployee(int id)
		{
			var employee = await _context.Employees
				.Include(e => e.ProjectEmployees)
					.ThenInclude(pe => pe.Projects)
				.FirstOrDefaultAsync(e => e.EmployeeId == id);

			if (employee == null)
			{
				return NotFound();
			}

			return employee;
		}

		// POST: api/Employees
		[HttpPost]
		public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_context.Employees.Add(employee);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
		}

		// PUT: api/Employees/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutEmployee(int id, Employee employee)
		{
			if (id != employee.EmployeeId)
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_context.Entry(employee).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!EmployeeExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		private bool EmployeeExists(int id)
		{
			throw new NotImplementedException();
		}

		// DELETE: api/Employees/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			var employee = await _context.Employees.FindAsync(id);
			if (employee == null)
			{
				return NotFound();
			}

			_context.Employees.Remove(employee);
			await _context.SaveChangesAsync();

			return NoContent();
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
