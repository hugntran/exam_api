namespace DMAWS_T2305M_TranHung.Models.Dto
{
	public class GetBasicEmployeeDTO
	{
		public int EmployeeId { get; set; }
		public string EmployeeName { get; set; }
		public DateTime EmployeeDOB { get; set; }
		public string EmployeeDepartment { get; set; }
	}

	public class GetDetailEmployeeDTO
	{
		public int EmployeeId { get; set; }
		public string EmployeeName { get; set; }
		public DateTime EmployeeDOB { get; set; }
		public string EmployeeDepartment { get; set; }
		public List<GetBasicProjectDTO> BasicProjectDTOs { get; set; }
	}

	public class EmployeeQueryParameters
	{
		public string? EmployeeName { get; set; }
		public DateTime? EmployeeDOBFromDate { get; set; }
		public DateTime? EmployeeDOBToDate { get; set; }
	}
}
