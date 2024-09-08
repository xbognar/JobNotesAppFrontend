using DataAccess.Models;
using JobNotesWPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IJobService
{
	
	Task<IEnumerable<Job>> GetJobsAsync();
	
	Task<Job> GetJobByIdAsync(int id);
	
	Task<Job> AddJobAsync(Job job);
	
	Task UpdateJobAsync(Job job);
	
	Task DeleteJobAsync(int id);
	
	Task<int> GetJobsCountForYearAsync(int year);
	
	Task<int> GetJobsCountForMonthAsync(int year, int month);

	Task<IEnumerable<Job>> SearchJobsAsync(string? location, string? clientName, string? notes);

}

