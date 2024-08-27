namespace JobNotesWPF.Models
{
	public class Job
	{
	
		public int Id { get; set; }
		
		public int SerialNumber { get; set; }
		
		public string JobNumber { get; set; }
		
		public string Location { get; set; }
		
		public string? ClientName { get; set; }
		
		public DateTime? MeasurementDate { get; set; }
		
		public string? Notes { get; set; }
		
		public bool IsCompleted { get; set; }
	
	}
}
