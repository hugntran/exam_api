namespace DMAWS_T2305M_TranHung.Models.Dto
{
	public class GetBasicProjectDTO
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public DateTime ProjectStartDate { get; set; }
		public DateTime? ProjectEndDate { get; set; }
	}
	public class GetDetailProjectDTO
	{
		public int ProjectId { get; set; }
		public string ProjectName { get; set; }
		public DateTime ProjectStartDate { get; set; }
		public DateTime? ProjectEndDate { get; set; }
		public List<GetBasicEmployeeDTO> Employees { get; set; }
	}

	public class ProjectQueryParameters
	{
		public bool? IsProjectInProgress { get; set; }
		public bool? IsProjectInFinished { get; set; }
		public string? ProjectName { get; set; }
	}
}
