using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_TranHung.Models
{
	public class ProjectEmployee
	{
		[Required]
		public int EmployeeId { get; set; }

		[Required]
		public int ProjectId { get; set; }

		[Required(ErrorMessage = "Tasks are required.")]
		public string Tasks { get; set; }

		public virtual Employee? Employees { get; set; }
		public virtual Project? Projects { get; set; }
	}
}
