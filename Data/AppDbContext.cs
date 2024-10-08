using Microsoft.EntityFrameworkCore;
namespace DMAWS_T2305M_TranHung.Models {

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Project> Projects { get; set; }
	public DbSet<Employee> Employees { get; set; }
	public DbSet<ProjectEmployee> ProjectEmployees { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectEmployee>()
			.HasKey(pe => new { pe.ProjectId, pe.EmployeeId });

		modelBuilder.Entity<ProjectEmployee>()
			.HasOne(pe => pe.Employees)
			.WithMany(e => e.ProjectEmployees)
			.HasForeignKey(pe => pe.EmployeeId);

		modelBuilder.Entity<ProjectEmployee>()
			.HasOne(pe => pe.Projects)
			.WithMany(p => p.ProjectEmployees)
			.HasForeignKey(pe => pe.ProjectId);
		}
	}
}