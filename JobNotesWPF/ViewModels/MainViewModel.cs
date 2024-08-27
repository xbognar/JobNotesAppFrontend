using CommunityToolkit.Mvvm.Input;
using JobNotesAppFrontend.Helpers;
using JobNotesWPF.Models;
using JobNotesWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class MainViewModel : BaseViewModel
{
	private readonly IJobService _jobService;
	private readonly IServiceProvider _serviceProvider;
	private readonly int _currentYear = DateTime.Now.Year;
	private readonly int _currentMonth = DateTime.Now.Month;

	private int _selectedYear;
	private int _selectedMonth;
	private string _searchLocation;
	private ObservableCollection<string> _filteredLocations;
	private string _selectedLocation;

	public ObservableCollection<Job> Jobs { get; private set; }
	public ObservableCollection<int> Years { get; private set; }
	public ObservableCollection<string> Months { get; private set; }
	public ObservableCollection<string> FilteredLocations
	{
		get => _filteredLocations;
		set => SetProperty(ref _filteredLocations, value);
	}

	public ICommand LoadJobsCommand { get; }
	public ICommand AddJobCommand { get; }
	public ICommand DeleteJobCommand { get; }
	public ICommand NavigateLeftCommand { get; }
	public ICommand NavigateRightCommand { get; }
	public ICommand SearchCommand { get; }
	public ICommand OpenJobListCommand { get; }

	private int _monthlyJobCount;
	public int MonthlyJobCount
	{
		get => _monthlyJobCount;
		set => SetProperty(ref _monthlyJobCount, value);
	}

	private int _yearlyJobCount;
	public int YearlyJobCount
	{
		get => _yearlyJobCount;
		set => SetProperty(ref _yearlyJobCount, value);
	}

	public int SelectedYear
	{
		get => _selectedYear;
		set
		{
			if (SetProperty(ref _selectedYear, value))
			{
				LoadJobs();
			}
		}
	}

	public int SelectedMonth
	{
		get => _selectedMonth;
		set
		{
			if (SetProperty(ref _selectedMonth, value))
			{
				LoadJobs();
			}
		}
	}

	public string SearchLocation
	{
		get => _searchLocation;
		set
		{
			if (SetProperty(ref _searchLocation, value))
			{
				UpdateFilteredLocationsAsync();
			}
		}
	}

	public string SelectedLocation
	{
		get => _selectedLocation;
		set
		{
			if (SetProperty(ref _selectedLocation, value))
			{
				if (string.IsNullOrEmpty(_selectedLocation))
				{
					LoadJobs(); // Reset to the default view if no location is selected
				}
				else
				{
					FilterJobsBySelectedLocationAsync();
				}
			}
		}
	}

	public MainViewModel(IJobService jobService, IServiceProvider serviceProvider)
	{
		_jobService = jobService;
		_serviceProvider = serviceProvider;
		Jobs = new ObservableCollection<Job>();

		Years = new ObservableCollection<int>(Enumerable.Range(2010, 41)); // Years from 2010 to 2050
		Months = new ObservableCollection<string>(new[]
		{
			"Január", "Február", "Marec", "Apríl", "Máj", "Jún",
			"Júl", "August", "September", "Október", "November", "December"
		});

		_filteredLocations = new ObservableCollection<string>();
		_selectedYear = _currentYear;
		_selectedMonth = _currentMonth;

		LoadJobsCommand = new RelayCommand(LoadJobs);
		AddJobCommand = new RelayCommand(AddJob);
		DeleteJobCommand = new RelayCommand<Job>(DeleteJob);
		NavigateLeftCommand = new RelayCommand(NavigateLeft);
		NavigateRightCommand = new RelayCommand(NavigateRight);
		SearchCommand = new RelayCommand(LoadJobs);
		OpenJobListCommand = new RelayCommand(OpenJobListWindow);

		LoadJobs();
	}

	private async void LoadJobs()
	{
		Jobs.Clear();
		//var jobs = await _jobService.GetJobsAsync();

		//foreach (var job in jobs)
		//{
		//	job.PropertyChanged += (s, e) =>
		//	{
		//		if (e.PropertyName == nameof(job.IsCompleted))
		//		{
		//			HandleJobCompletionChanged(job);
		//		}
		//	};
		//	Jobs.Add(job);
		//}

		UpdateJobCounts();
	}

	private async void HandleJobCompletionChanged(Job job)
	{
		if (job.IsCompleted)
		{
			var result = MessageBox.Show("Are you sure you want to mark this job as completed?", "Confirm Completion", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.No)
			{
				job.IsCompleted = false; // Revert if the user cancels
			}
			else
			{
				await _jobService.UpdateJobAsync(job);
			}
		}
		else
		{
			var result = MessageBox.Show("Are you sure you want to unmark this job as completed?", "Confirm", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.No)
			{
				job.IsCompleted = true; // Revert if the user cancels
			}
		}
	}

	private async Task UpdateFilteredLocationsAsync()
	{
		if (string.IsNullOrEmpty(_searchLocation))
		{
			FilteredLocations.Clear();
			return;
		}

		var distinctLocations = await _jobService.SearchJobsAsync(_searchLocation, null, null);
		var locationsList = distinctLocations.Select(j => j.Location).Distinct().ToList();

		FilteredLocations.Clear();
		foreach (var location in locationsList)
		{
			FilteredLocations.Add(location);
		}
	}

	private async Task FilterJobsBySelectedLocationAsync()
	{
		var filteredJobs = await _jobService.SearchJobsAsync(_selectedLocation, null, null);

		Jobs.Clear();
		foreach (var job in filteredJobs)
		{
			Jobs.Add(job);
		}
	}

	private async void AddJob()
	{
		var newJob = new Job { IsCompleted = false };
		await _jobService.AddJobAsync(newJob);
		Jobs.Add(newJob);
		UpdateJobCounts();
	}

	private async void DeleteJob(Job job)
	{
		if (job == null) return;

		if (MessageBox.Show("Are you sure you want to delete this job?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
		{
			await _jobService.DeleteJobAsync(job.Id);
			Jobs.Remove(job);
			UpdateJobCounts();
		}
	}

	private void NavigateLeft()
	{
		if (_selectedMonth == 1)
		{
			if (_selectedYear > 2010)
			{
				SelectedYear--;
				SelectedMonth = 12;
			}
		}
		else
		{
			SelectedMonth--;
		}

		LoadJobs();
	}

	private void NavigateRight()
	{
		if (_selectedMonth == 12)
		{
			if (_selectedYear < 2050)
			{
				SelectedYear++;
				SelectedMonth = 1;
			}
		}
		else
		{
			SelectedMonth++;
		}

		LoadJobs();
	}

	private void UpdateJobCounts()
	{
		MonthlyJobCount = Jobs.Count(j => j.MeasurementDate?.Year == _selectedYear && j.MeasurementDate?.Month == _selectedMonth);
		YearlyJobCount = Jobs.Count(j => j.MeasurementDate?.Year == _selectedYear);
	}

	private void OpenJobListWindow()
	{
		var jobListWindow = _serviceProvider.GetRequiredService<JobListWindow>();
		jobListWindow.Show();
	}
}
