using DMAWS_T2305M_TranHung.Data;
using DMAWS_T2305M_TranHung.Models;
using DMAWS_T2305M_TranHung.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAWS_T2305M_TranHung.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectsController : ControllerBase
	{
		readonly private T2305mApiContext _context;
		public ProjectsController(T2305mApiContext context)
		{
			_context = context;
		}

		// GET: api/Projects
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
		{
			return await _context.Projects.ToListAsync();
		}

		// GET: api/Projects/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Project>> GetProject(int id)
		{
			var project = await _context.Projects
				.Include(p => p.ProjectEmployees)
					.ThenInclude(pe => pe.Employees)
				.FirstOrDefaultAsync(p => p.ProjectId == id);

			if (project == null)
			{
				return NotFound();
			}

			return project;
		}

		// POST: api/Projects
		[HttpPost]
		public async Task<ActionResult<Project>> PostProject(Project project)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_context.Projects.Add(project);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetProject", new { id = project.ProjectId }, project);
		}

		// PUT: api/Projects/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProject(int id, Project project)
		{
			if (id != project.ProjectId)
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_context.Entry(project).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProjectExists(id))
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

		private bool ProjectExists(int id)
		{
			throw new NotImplementedException();
		}

		// DELETE: api/Projects/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProject(int id)
		{
			var project = await _context.Projects.FindAsync(id);
			if (project == null)
			{
				return NotFound();
			}

			_context.Projects.Remove(project);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpGet("GetProjects")]
		public async Task<ActionResult<IEnumerable<GetBasicProjectDTO>>> GetBasicProjectDTOs([FromQuery] ProjectQueryParameters queryParameters)
		{
			try
			{
				// Query the database to get the list of projects
				var query = _context.Projects.AsQueryable();

				// Filtering by Project Name if provided
				if (!string.IsNullOrWhiteSpace(queryParameters.ProjectName))
				{
					query = query.Where(p => p.ProjectName.Contains(queryParameters.ProjectName));
				}

				// Filtering by project progress status
				if (queryParameters.IsProjectInProgress.HasValue)
				{
					query = query.Where(p => !p.ProjectEndDate.HasValue || p.ProjectEndDate > DateTime.Now);

				}
				if (queryParameters.IsProjectInFinished.HasValue)
				{
					query = query.Where(p => p.ProjectEndDate.HasValue && p.ProjectEndDate <= DateTime.Now);
				}

				// Fetching the projects and mapping to GetBasicProjectDTO
				var projects = await query
					.Select(p => new GetBasicProjectDTO
					{
						ProjectId = p.ProjectId,
						ProjectName = p.ProjectName,
						ProjectStartDate = p.ProjectStartDate,
						ProjectEndDate = p.ProjectEndDate
					})
					.ToListAsync();

				// Return the filtered list of projects
				return Ok(projects);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}


		[HttpGet("GetDetailProject/{projectId}")]
		public async Task<ActionResult<GetDetailProjectDTO>> GetDetailProjectDTO(int projectId)
		{
			try
			{
				// Eagerly load the related employees to avoid multiple database calls (N+1 problem)
				var project = await _context.Projects
					.Include(p => p.ProjectEmployees)
					.ThenInclude(pe => pe.Employees)
					.FirstOrDefaultAsync(p => p.ProjectId == projectId);

				// Check if the project exists
				if (project == null)
				{
					return NotFound($"Project with ID {projectId} not found.");
				}

				// Map the project to the GetDetailProjectDTO
				var projectDetails = new GetDetailProjectDTO
				{
					ProjectId = project.ProjectId,
					ProjectName = project.ProjectName,
					ProjectStartDate = project.ProjectStartDate,
					ProjectEndDate = project.ProjectEndDate,
					Employees = project.ProjectEmployees?.Select(pe => new GetBasicEmployeeDTO
					{
						EmployeeId = pe.Employees?.EmployeeId ?? 0,  // Null check
						EmployeeName = pe.Employees?.EmployeeName ?? "Unknown"  // Null check
					}).ToList() ?? new List<GetBasicEmployeeDTO>()  // Handle null collection
				};

				// Return the project details
				return Ok(projectDetails);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

	}
}
