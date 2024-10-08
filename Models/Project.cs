using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_TranHung.Models
{
	public class Project
	{
		[Key] // Primary key
		public int ProjectId { get; set; }

		[Required(ErrorMessage = "Project Name is required.")]
		[StringLength(150, MinimumLength = 2, ErrorMessage = "Project Name must be between 2 and 150 characters.")]
		public string ProjectName { get; set; }

		[Required(ErrorMessage = "Project Start Date is required.")]
		public DateTime ProjectStartDate { get; set; }

		[DataType(DataType.Date)]
		[CompareDates(nameof(ProjectStartDate), ErrorMessage = "Project Start Date must be earlier than Project End Date.")]
		public DateTime? ProjectEndDate { get; set; }

		public virtual ICollection<ProjectEmployee>? ProjectEmployees { get; set; }
	}

	// Custom Validation Attribute for Project Start/End Dates
	public class CompareDatesAttribute : ValidationAttribute
	{
		private readonly string _startDatePropertyName;

		public CompareDatesAttribute(string startDatePropertyName)
		{
			_startDatePropertyName = startDatePropertyName;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var endDate = (DateTime?)value;
			var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
			var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);

			if (endDate.HasValue && endDate.Value < startDate)
			{
				return new ValidationResult(ErrorMessage);
			}

			return ValidationResult.Success;
		}
	}
}
