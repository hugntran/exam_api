using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_TranHung.Models
{
	public class Employee
	{
		[Key] // Primary key
		public int EmployeeId { get; set; }

		[Required(ErrorMessage = "Employee Name is required.")]
		[StringLength(150, MinimumLength = 2, ErrorMessage = "Employee Name must be between 2 and 150 characters.")]
		public string EmployeeName { get; set; }

		[Required(ErrorMessage = "Employee Date of Birth is required.")]
		[DataType(DataType.Date)]
		[MinAge(16, ErrorMessage = "Employee must be at least 16 years old.")]
		public DateTime EmployeeDOB { get; set; }

		[Required(ErrorMessage = "Employee Department is required.")]
		public string EmployeeDepartment { get; set; }

		public virtual ICollection<ProjectEmployee>? ProjectEmployees { get; set; }
	}

	// Custom Validation Attribute for Minimum Age
	public class MinAgeAttribute : ValidationAttribute
	{
		private readonly int _minimumAge;

		public MinAgeAttribute(int minimumAge)
		{
			_minimumAge = minimumAge;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var birthDate = (DateTime)value;
			if (birthDate.AddYears(_minimumAge) > DateTime.Today)
			{
				return new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
		}
	}
}
