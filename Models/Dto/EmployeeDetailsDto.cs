// ~/Models/Dto/EmployeeDetailsDto.cs
using System;
using System.Collections.Generic;

public class EmployeeDetailsDto
{
	public int EmployeeId { get; set; } // Id nhân viên
	public string EmployeeName { get; set; } // Tên nhân viên
	public DateTime EmployeeDOB { get; set; } // Ngày tháng năm sinh
	public string EmployeeDepartment { get; set; } // Bộ phận
	public List<ProjectDto> Projects { get; set; } // Danh sách các dự án
}
