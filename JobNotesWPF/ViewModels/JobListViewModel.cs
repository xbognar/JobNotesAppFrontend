//using JobNotesAppFrontend.Helpers;
//using JobNotesWPF.Models;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Threading.Tasks;

//namespace JobNotesWPF.ViewModels
//{
//	public class JobListViewModel : BaseViewModel
//	{
//		private ObservableCollection<Job> _jobs;
//		public ObservableCollection<Job> Jobs
//		{
//			get => _jobs;
//			set => SetProperty(ref _jobs, value);
//		}

//		private int _totalJobs;
//		public int TotalJobs
//		{
//			get => _totalJobs;
//			set => SetProperty(ref _totalJobs, value);
//		}

//		private int _bestYear;
//		public int BestYear
//		{
//			get => _bestYear;
//			set => SetProperty(ref _bestYear, value);
//		}

//		private string _bestMonth;
//		public string BestMonth
//		{
//			get => _bestMonth;
//			set => SetProperty(ref _bestMonth, value);
//		}

//		private string _bestMonthYear;
//		public string BestMonthYear
//		{
//			get => _bestMonthYear;
//			set => SetProperty(ref _bestMonthYear, value);
//		}

//		private double _averageJobsPerYear;
//		public double AverageJobsPerYear
//		{
//			get => _averageJobsPerYear;
//			set => SetProperty(ref _averageJobsPerYear, value);
//		}

//		private readonly IJobService _jobService;

//		public JobListViewModel(IJobService jobService)
//		{
//			_jobService = jobService;
//			Jobs = new ObservableCollection<Job>();

//			// Initialize data asynchronously
//			LoadData();
//		}

//		private async void LoadData()
//		{
//			var allJobs = await _jobService.GetJobsAsync();
//			Jobs = new ObservableCollection<Job>(allJobs);
//			CalculateStatistics(allJobs);
//		}

//		private void CalculateStatistics(IEnumerable<Job> jobs)
//		{
//			TotalJobs = jobs.Count();

//			if (TotalJobs == 0)
//			{
//				BestYear = 0;
//				BestMonth = "N/A";
//				BestMonthYear = "N/A";
//				AverageJobsPerYear = 0;
//				return;
//			}

//			// Calculate best year
//			var yearGroups = jobs.GroupBy(j => j.MeasurementDate.Value.Year)
//				.Select(g => new { Year = g.Key, Count = g.Count() })
//				.OrderByDescending(g => g.Count)
//				.ToList();

//			BestYear = yearGroups.First().Year;

//			// Calculate best month
//			var monthGroups = jobs.GroupBy(j => new { j.MeasurementDate.Value.Year, j.MeasurementDate.Value.Month })
//				.Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
//				.OrderByDescending(g => g.Count)
//				.ToList();

//			var bestMonthGroup = monthGroups.First();
//			BestMonth = new DateTime(1, bestMonthGroup.Month, 1).ToString("MMMM");
//			BestMonthYear = bestMonthGroup.Year.ToString();

//			// Calculate average jobs per year
//			AverageJobsPerYear = yearGroups.Average(g => g.Count);
//		}
//	}
//}


using JobNotesAppFrontend.Helpers;
using JobNotesWPF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JobNotesWPF.ViewModels
{
	public class JobListViewModel : BaseViewModel
	{
		private ObservableCollection<JobViewModel> _jobs;
		public ObservableCollection<JobViewModel> Jobs
		{
			get => _jobs;
			set => SetProperty(ref _jobs, value);
		}

		private int _totalJobs;
		public int TotalJobs
		{
			get => _totalJobs;
			set => SetProperty(ref _totalJobs, value);
		}

		private int _bestYear;
		public int BestYear
		{
			get => _bestYear;
			set => SetProperty(ref _bestYear, value);
		}

		private string _bestMonth;
		public string BestMonth
		{
			get => _bestMonth;
			set => SetProperty(ref _bestMonth, value);
		}

		private string _bestMonthYear;
		public string BestMonthYear
		{
			get => _bestMonthYear;
			set => SetProperty(ref _bestMonthYear, value);
		}

		private double _averageJobsPerYear;
		public double AverageJobsPerYear
		{
			get => _averageJobsPerYear;
			set => SetProperty(ref _averageJobsPerYear, value);
		}

		private readonly IJobService _jobService;

		public JobListViewModel(IJobService jobService)
		{
			_jobService = jobService;
			Jobs = new ObservableCollection<JobViewModel>();

			// Initialize data asynchronously
			LoadData();
		}

		private async void LoadData()
		{
			var allJobs = await _jobService.GetJobsAsync();
			Jobs = new ObservableCollection<JobViewModel>(allJobs.Select(j => new JobViewModel(j)));
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

	// This class wraps each Job with a computed property for "Ano" / "Nie"
	public class JobViewModel : BaseViewModel
	{
		private readonly Job _job;

		public JobViewModel(Job job)
		{
			_job = job;
		}

		public int SerialNumber => _job.SerialNumber;
		public string JobNumber => _job.JobNumber;
		public string Location => _job.Location;
		public string ClientName => _job.ClientName;
		public DateTime? MeasurementDate => _job.MeasurementDate;
		public string Notes => _job.Notes;

		// New computed property for displaying "Ano" or "Nie"
		public string CompletedStatus => _job.IsCompleted ? "Áno" : "Nie";
	}
}
