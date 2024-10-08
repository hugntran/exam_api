using DMAWS_T2305M_TranHung.Models;
using Microsoft.EntityFrameworkCore;

namespace DMAWS_T2305M_TranHung.Data;

public partial class T2305mApiContext : DbContext
{
	public static string ConnectionString;
	public T2305mApiContext()
	{
	}

	public T2305mApiContext(DbContextOptions<T2305mApiContext> options)
		: base(options)
	{
	}

	public DbSet<Project> Projects { get; set; }
	public DbSet<Employee> Employees { get; set; }
	public DbSet<ProjectEmployee> ProjectEmployees { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
		=> optionsBuilder.UseSqlServer(ConnectionString);

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProjectEmployee>()
			  .HasKey(pe => new { pe.EmployeeId, pe.ProjectId }); // Composite key

		modelBuilder.Entity<ProjectEmployee>()
			.HasOne(pe => pe.Employees)
			.WithMany(e => e.ProjectEmployees)
			.HasForeignKey(pe => pe.EmployeeId);

		modelBuilder.Entity<ProjectEmployee>()
			.HasOne(pe => pe.Projects)
			.WithMany(p => p.ProjectEmployees)
			.HasForeignKey(pe => pe.ProjectId);


		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
