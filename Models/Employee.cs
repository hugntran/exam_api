using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMAWS_T2305M_TranHung.Models
{
	public class Employee
	{
		public int EmployeeId { get; set; } // Id nhân viên

		[Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
		[StringLength(150, MinimumLength = 2, ErrorMessage = "Tên nhân viên phải có độ dài từ 2 đến 150 ký tự")]
		public string EmployeeName { get; set; } // Tên nhân viên

		[Required(ErrorMessage = "Ngày sinh là bắt buộc")]
		public DateTime EmployeeDOB { get; set; } // Ngày tháng năm sinh

		[Required(ErrorMessage = "Bộ phận là bắt buộc")]
		public string EmployeeDepartment { get; set; } // Bộ phận

		[Required(ErrorMessage = "Email là bắt buộc")]
		[EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
		public string Email { get; set; } // Địa chỉ email

		// Liên kết với ProjectEmployee
		public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }

		// Custom validation: Employee must be over 16 years old
		public bool IsValidEmployeeDOB()
		{
			var today = DateTime.Today;
			var age = today.Year - EmployeeDOB.Year;
			if (EmployeeDOB > today.AddYears(-age)) age--;

			return age >= 16;
		}
	}
}