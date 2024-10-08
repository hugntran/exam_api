using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMAWS_T2305M_TranHung.Models
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly AppDbContext _context;

		public EmployeesController(AppDbContext context)
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
			if (!employee.IsValidEmployeeDOB())
			{
				ModelState.AddModelError("EmployeeDOB", "Nhân viên phải trên 16 tuổi.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_context.Employees.Add(employee);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
		}

		// PUT: api/Employees/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutEmployee(int id, Employee employee)
		{
			if (id != employee.EmployeeId)
			{
				return BadRequest();
			}

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

		// Search employees by name and DOB range
		[HttpGet("Search")]
		public async Task<ActionResult<IEnumerable<Employee>>> SearchEmployees(string employeeName, DateTime? dobFrom, DateTime? dobTo)
		{
			var employees = _context.Employees.AsQueryable();

			if (!string.IsNullOrEmpty(employeeName))
			{
				employees = employees.Where(e => e.EmployeeName.Contains(employeeName));
			}

			if (dobFrom.HasValue)
			{
				employees = employees.Where(e => e.EmployeeDOB >= dobFrom.Value);
			}

			if (dobTo.HasValue)
			{
				employees = employees.Where(e => e.EmployeeDOB <= dobTo.Value);
			}

			return await employees.ToListAsync();
		}

		// ~/Controllers/EmployeesController.cs
		[HttpGet("{id}/projects")]
		public async Task<ActionResult<EmployeeDetailsDto>> GetEmployeeDetailsWithProjects(int id)
		{
			// Lấy thông tin nhân viên cùng với các dự án của họ
			var employee = await _context.Employees
				.Include(e => e.ProjectEmployees)
				.ThenInclude(pe => pe.Projects) // Liên kết để lấy các dự án của nhân viên
				.FirstOrDefaultAsync(e => e.EmployeeId == id);

			if (employee == null)
			{
				return NotFound();
			}

			// Tạo DTO để trả về dữ liệu chi tiết về nhân viên và các dự án của họ
			var employeeDetails = new EmployeeDetailsDto
			{
				EmployeeId = employee.EmployeeId,
				EmployeeName = employee.EmployeeName,
				EmployeeDOB = employee.EmployeeDOB,
				EmployeeDepartment = employee.EmployeeDepartment,
				Projects = employee.ProjectEmployees.Select(pe => new ProjectDto
				{
					ProjectId = pe.ProjectId,
					ProjectName = pe.Projects.ProjectName,
					ProjectStartDate = pe.Projects.ProjectStartDate,
					ProjectEndDate = pe.Projects.ProjectEndDate
				}).ToList()
			};

			return Ok(employeeDetails);
		}


		private bool EmployeeExists(int id)
		{
			return _context.Employees.Any(e => e.EmployeeId == id);
		}
	}
}