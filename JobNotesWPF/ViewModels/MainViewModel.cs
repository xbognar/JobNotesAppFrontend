using CommunityToolkit.Mvvm.Input;
using JobNotesAppFrontend.Helpers;
using JobNotesWPF.Models;
using JobNotesWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
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
	private readonly string _currentMonth = DateTime.Now.ToString("MMMM");

	private int _selectedYear;
	private string _selectedMonth;
	private string _searchText;
	private string _userNote1;
	private string _userNote2;

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

	public int SelectedYear
	{
		get => _selectedYear;
		set => SetProperty(ref _selectedYear, value);
	}

	public string SelectedMonth
	{
		get => _selectedMonth;
		set => SetProperty(ref _selectedMonth, value);
	}

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

	public string UserNote1
	{
		get => _userNote1;
		set
		{
			SetProperty(ref _userNote1, value);
			SaveUserNotes();
		}
	}

	public string UserNote2
	{
		get => _userNote2;
		set
		{
			SetProperty(ref _userNote2, value);
			SaveUserNotes();
		}
	}

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (SetProperty(ref _searchText, value))
			{
				SearchJobs();
			}
		}
	}

	public ICommand LoadJobsCommand { get; }
	public ICommand AddJobCommand { get; }
	public ICommand DeleteJobCommand { get; }
	public ICommand UpdateJobCommand { get; }
	public ICommand NavigateLeftCommand { get; }
	public ICommand NavigateRightCommand { get; }
	public ICommand SearchCommand { get; }
	public ICommand OpenJobListCommand { get; }
	public ICommand LogoutCommand { get; }
	public ICommand ExitCommand { get; }
	public ICommand HandleCompletionCheckboxCommand { get; }

	public MainViewModel(IJobService jobService, IServiceProvider serviceProvider)
	{
		_jobService = jobService;
		_serviceProvider = serviceProvider;

		SelectedYear = _currentYear;
		SelectedMonth = _currentMonth;

		Jobs = new ObservableCollection<Job>();
		Years = new ObservableCollection<int>(Enumerable.Range(2020, 31));
		Months = new ObservableCollection<string>(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.MonthNames
													.Where(m => !string.IsNullOrEmpty(m))
													.ToArray());
		_filteredLocations = new ObservableCollection<string>();

		LoadJobsCommand = new RelayCommand(LoadJobs);
		AddJobCommand = new RelayCommand(AddJob);
		DeleteJobCommand = new RelayCommand<Job>(DeleteJob);
		UpdateJobCommand = new RelayCommand<Job>(async job => await UpdateJob(job));
		NavigateLeftCommand = new RelayCommand(NavigateLeft);
		NavigateRightCommand = new RelayCommand(NavigateRight);
		SearchCommand = new RelayCommand(SearchJobs);
		OpenJobListCommand = new RelayCommand(OpenJobListWindow);
		LogoutCommand = new RelayCommand(Logout);
		HandleCompletionCheckboxCommand = new RelayCommand<Job>(async job => await HandleCompletionCheckboxChange(job));
		ExitCommand = new RelayCommand(ExitApplication);
	}

	public void Initialize()
	{
		LoadUserNotes();
		LoadJobs();
		UpdateJobCounts();
	}

	private async void SearchJobs()
	{
		if (string.IsNullOrWhiteSpace(SearchText))
		{
			LoadJobs();
		}
		else
		{
			Jobs.Clear();
			var allJobs = await _jobService.GetJobsAsync();
			var filteredJobs = allJobs.Where(j => j.Location != null && j.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

			foreach (var job in filteredJobs)
			{
				Jobs.Add(job);
			}
		}
	}

	private async void LoadJobs()
	{
		Jobs.Clear();
		var allJobs = await _jobService.GetJobsAsync();
		var jobsForSelectedDate = allJobs.Where(j => j.MeasurementDate.HasValue
													 && j.MeasurementDate.Value.Year == SelectedYear
													 && j.MeasurementDate.Value.ToString("MMMM") == SelectedMonth).ToList();

		foreach (var job in jobsForSelectedDate)
		{
			Jobs.Add(job);
		}
		UpdateJobCounts();
	}

	private async void AddJob()
	{
		var newSerialNumber = await GetNextSerialNumber();

		var newJob = new Job
		{
			SerialNumber = newSerialNumber,
			JobNumber = "null",
			Location = "null",
			ClientName = "null",
			MeasurementDate = DateTime.Today,
			Notes = "null",
			IsCompleted = false
		};

		await _jobService.AddJobAsync(newJob);
		Jobs.Add(newJob);
		UpdateJobCounts();
	}

	private async Task<int> GetNextSerialNumber()
	{
		var currentYear = DateTime.Now.Year;
		var allJobs = await _jobService.GetJobsAsync();

		var currentYearJobs = allJobs.Where(j => j.MeasurementDate.HasValue && j.MeasurementDate.Value.Year == currentYear);

		var maxSerialNumber = currentYearJobs.Any() ? currentYearJobs.Max(j => j.SerialNumber) : 0;

		return maxSerialNumber + 1;
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

	private async Task UpdateJob(Job job)
	{
		try
		{
			await _jobService.UpdateJobAsync(job);
			MessageBox.Show("Job updated successfully!");
			
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to update job: {ex.Message}");
		}
	}


	public async Task HandleCompletionCheckboxChange(Job job)
	{
		if (job.IsCompleted)
		{
			var result = MessageBox.Show("Mark this job as completed?", "Confirm", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				job.IsCompleted = true;
				await UpdateJob(job);
			}
			else
			{
				job.IsCompleted = false;
			}
		}
		else
		{
			var result = MessageBox.Show("Reopen this job for editing?", "Confirm", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				job.IsCompleted = false;
				await UpdateJob(job);
			}
			else
			{
				job.IsCompleted = true;
			}
		}
	}


	private void NavigateLeft()
	{
		int monthIndex = Array.IndexOf(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.MonthNames, _selectedMonth) + 1;

		if (monthIndex == 1)
		{
			if (_selectedYear > Years.Min())
			{
				SelectedYear--;
				SelectedMonth = "December";
			}
		}
		else
		{
			SelectedMonth = Months[monthIndex - 2];
		}

		LoadJobs();
	}

	private void NavigateRight()
	{
		int monthIndex = Array.IndexOf(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.MonthNames, _selectedMonth) + 1;

		if (monthIndex == 12)
		{
			if (_selectedYear < Years.Max())
			{
				SelectedYear++;
				SelectedMonth = "January";
			}
		}
		else
		{
			SelectedMonth = Months[monthIndex];
		}

		LoadJobs();
	}

	private void UpdateJobCounts()
	{
		MonthlyJobCount = Jobs.Count(j => j.MeasurementDate?.Year == SelectedYear && j.MeasurementDate?.ToString("MMMM") == SelectedMonth);
		YearlyJobCount = Jobs.Count(j => j.MeasurementDate?.Year == SelectedYear);
	}

	private void OpenJobListWindow()
	{
		var jobListWindow = _serviceProvider.GetRequiredService<JobListWindow>();
		jobListWindow.Show();
	}

	private void Logout()
	{
		if (MessageBox.Show("Are you sure you want to log out and exit the application?", "Logout", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
		{
			ExitApplication();
		}
	}

	private void ExitApplication()
	{
		Application.Current.Shutdown();
	}

	private void LoadUserNotes()
	{
		UserNote1 = JobNotesWPF.Properties.Settings.Default.UserNote1;
		UserNote2 = JobNotesWPF.Properties.Settings.Default.UserNote2;
	}

	private void SaveUserNotes()
	{
		JobNotesWPF.Properties.Settings.Default.UserNote1 = UserNote1;
		JobNotesWPF.Properties.Settings.Default.UserNote2 = UserNote2;
		JobNotesWPF.Properties.Settings.Default.Save();
	}
}
