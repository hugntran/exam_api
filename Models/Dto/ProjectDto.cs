// ~/Models/Dto/ProjectDto.cs
using System;

public class ProjectDto
{
	public int ProjectId { get; set; } // Id dự án
	public string ProjectName { get; set; } // Tên dự án
	public DateTime ProjectStartDate { get; set; } // Ngày bắt đầu
	public DateTime? ProjectEndDate { get; set; } // Ngày kết thúc (nullable)
}