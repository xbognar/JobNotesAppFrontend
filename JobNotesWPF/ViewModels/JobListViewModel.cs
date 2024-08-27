using JobNotesAppFrontend.Helpers;
using JobNotesWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JobNotesWPF.ViewModels
{
	public class JobListViewModel : BaseViewModel
	{
		private ObservableCollection<Job> _jobs;
		public ObservableCollection<Job> Jobs
		{
			get => _jobs;
			set => SetProperty(ref _jobs, value);
		}

		public int TotalJobs { get; private set; }
		public int BestYear { get; private set; }
		public string BestMonth { get; private set; }
		public string BestMonthYear { get; private set; }
		public double AverageJobsPerYear { get; private set; }

		public JobListViewModel(IJobService jobService)
		{
			var allJobs = jobService.GetJobsAsync().Result;

			Jobs = new ObservableCollection<Job>(allJobs);

			CalculateStatistics(allJobs);
		}

		private void CalculateStatistics(IEnumerable<Job> jobs)
		{
			TotalJobs = jobs.Count();

			if (TotalJobs == 0)
			{
				BestYear = 0;
				BestMonth = "N/A";
				BestMonthYear = "N/A";
				AverageJobsPerYear = 0;
				return;
			}

			// Calculate best year
			var yearGroups = jobs.GroupBy(j => j.MeasurementDate.Value.Year)
				.Select(g => new { Year = g.Key, Count = g.Count() })
				.OrderByDescending(g => g.Count)
				.ToList();

			BestYear = yearGroups.First().Year;

			// Calculate best month
			var monthGroups = jobs.GroupBy(j => new { j.MeasurementDate.Value.Year, j.MeasurementDate.Value.Month })
				.Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
				.OrderByDescending(g => g.Count)
				.ToList();

			var bestMonthGroup = monthGroups.First();
			BestMonth = new DateTime(1, bestMonthGroup.Month, 1).ToString("MMMM");
			BestMonthYear = bestMonthGroup.Year.ToString();

			// Calculate average jobs per year
			AverageJobsPerYear = yearGroups.Average(g => g.Count);
		}
	}
}
