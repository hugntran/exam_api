using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DMAWS_T2305M_TranHung.Models
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ProjectsController(AppDbContext context)
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
			var project = await _context.Projects.FindAsync(id);
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
			if (!project.IsValidProjectDates())
			{
				ModelState.AddModelError("ProjectDates", "Ngày bắt đầu phải nhỏ hơn ngày kết thúc (nếu có).");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_context.Projects.Add(project);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
		}

		// PUT: api/Projects/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutProject(int id, Project project)
		{
			if (id != project.ProjectId)
			{
				return BadRequest();
			}

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

		// GET: api/Projects/Search
		[HttpGet("Search")]
		public async Task<ActionResult<IEnumerable<Project>>> SearchProjects(string projectName, bool? inProgress)
		{
			var projects = _context.Projects.AsQueryable();

			if (!string.IsNullOrEmpty(projectName))
			{
				projects = projects.Where(p => p.ProjectName.Contains(projectName));
			}

			if (inProgress.HasValue)
			{
				if (inProgress.Value)
				{
					projects = projects.Where(p => !p.ProjectEndDate.HasValue || p.ProjectEndDate > DateTime.Now);
				}
				else
				{
					projects = projects.Where(p => p.ProjectEndDate.HasValue && p.ProjectEndDate <= DateTime.Now);
				}
			}

			return await projects.ToListAsync();
		}

		// GET: api/Projects/Details/5
		[HttpGet("Details/{id}")]
		public async Task<ActionResult<Project>> GetProjectDetails(int id)
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

		private bool ProjectExists(int id)
		{
			return _context.Projects.Any(e => e.ProjectId == id);
		}
	}
}