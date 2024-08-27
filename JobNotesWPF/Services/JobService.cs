using JobNotesWPF.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class JobService : BaseService, IJobService
{
	public JobService(HttpClient httpClient) : base(httpClient) { }

	public async Task<IEnumerable<Job>> GetJobsAsync()
	{
		var response = await _httpClient.GetAsync("api/jobs");
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<IEnumerable<Job>>();
	}

	public async Task<Job> GetJobByIdAsync(int id)
	{
		var response = await _httpClient.GetAsync($"api/jobs/{id}");
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<Job>();
	}

	public async Task<Job> AddJobAsync(Job job)
	{
		var response = await _httpClient.PostAsJsonAsync("api/jobs", job);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<Job>();
	}

	public async Task UpdateJobAsync(Job job)
	{
		var response = await _httpClient.PutAsJsonAsync($"api/jobs/{job.Id}", job);
		response.EnsureSuccessStatusCode();
	}

	public async Task DeleteJobAsync(int id)
	{
		var response = await _httpClient.DeleteAsync($"api/jobs/{id}");
		response.EnsureSuccessStatusCode();
	}

	public async Task<int> GetJobsCountForYearAsync(int year)
	{
		var response = await _httpClient.GetAsync($"api/jobs/count/year/{year}");
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<int>();
	}

	public async Task<int> GetJobsCountForMonthAsync(int year, int month)
	{
		var response = await _httpClient.GetAsync($"api/jobs/count/year/{year}/month/{month}");
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<int>();
	}

	public async Task<IEnumerable<Job>> SearchJobsAsync(string? location, string? clientName, string? notes)
	{
		var query = $"api/jobs/search?location={location}&clientName={clientName}&notes={notes}";
		var response = await _httpClient.GetAsync(query);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadAsAsync<IEnumerable<Job>>();
	}
}
