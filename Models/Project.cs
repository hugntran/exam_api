// ~/Models/Project.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_TranHung.Models
{
	public class Project
	{
		public int ProjectId { get; set; } // Id thể loại

		[Required(ErrorMessage = "Tên dự án là bắt buộc")]
		[StringLength(150, MinimumLength = 2, ErrorMessage = "Tên dự án phải có độ dài từ 2 đến 150 ký tự")]
		public string ProjectName { get; set; } // Tên thể loại

		[Required(ErrorMessage = "Ngày bắt đầu dự án là bắt buộc")]
		public DateTime ProjectStartDate { get; set; } // Ngày bắt đầu

		public DateTime? ProjectEndDate { get; set; } // Ngày kết thúc (Nullable)

		// Liên kết với ProjectEmployee
		public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }

		// Custom validation rule: StartDate must be earlier than EndDate
		public bool IsValidProjectDates()
		{
			return !ProjectEndDate.HasValue || ProjectStartDate < ProjectEndDate.Value;
		}
	}
}