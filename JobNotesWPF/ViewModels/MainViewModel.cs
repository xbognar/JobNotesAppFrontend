using CommunityToolkit.Mvvm.Input;
using JobNotesAppFrontend.Helpers;
using JobNotesWPF.Models;
using JobNotesWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

public class MainViewModel : BaseViewModel
{
	private readonly IJobService _jobService;
	private readonly IAuthenticationService _authService;
	private readonly IServiceProvider _serviceProvider;
	private readonly int _currentYear = DateTime.Now.Year;
	private readonly int _currentMonth = DateTime.Now.Month;

	private int _selectedYear;
	private int _selectedMonth;
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

	public int SelectedYear
	{
		get => _selectedYear;
		set
		{
			if (SetProperty(ref _selectedYear, value))
			{
				LoadJobs(); // Reload jobs based on the selected year
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
				LoadJobs(); // Reload jobs based on the selected month
			}
		}
	}

	public string SearchText
	{
		get => _searchText;
		set
		{
			if (SetProperty(ref _searchText, value))
			{
				SearchJobs(); // Trigger search logic whenever the search text changes
			}
		}
	}

	public ICommand LoadJobsCommand { get; }
	public ICommand AddJobCommand { get; }
	public ICommand DeleteJobCommand { get; }
	public ICommand NavigateLeftCommand { get; }
	public ICommand NavigateRightCommand { get; }
	public ICommand SearchCommand { get; }
	public ICommand OpenJobListCommand { get; }
	public ICommand LogoutCommand { get; }
	public ICommand ExitCommand { get; }

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

	public MainViewModel(IJobService jobService, IAuthenticationService authService, IServiceProvider serviceProvider)
	{
		_jobService = jobService;
		_authService = authService;
		_serviceProvider = serviceProvider;
		Jobs = new ObservableCollection<Job>();

		Years = new ObservableCollection<int>(Enumerable.Range(2020, 31));
		Months = new ObservableCollection<string>(new[]
		{
			"Január", "Február", "Marec", "Apríl", "Máj", "Jún",
			"Júl", "August", "September", "Október", "November", "December"
		});

		SelectedYear = _currentYear;
		SelectedMonth = _currentMonth;

		_filteredLocations = new ObservableCollection<string>();

		LoadJobsCommand = new RelayCommand(LoadJobs);
		AddJobCommand = new RelayCommand(AddJob);
		DeleteJobCommand = new RelayCommand<Job>(DeleteJob);
		NavigateLeftCommand = new RelayCommand(NavigateLeft);
		NavigateRightCommand = new RelayCommand(NavigateRight);
		SearchCommand = new RelayCommand(SearchJobs);
		OpenJobListCommand = new RelayCommand(OpenJobListWindow);
		LogoutCommand = new RelayCommand(Logout);
		ExitCommand = new RelayCommand(ExitApplication);

		LoadUserNotes();
	}

	public async Task Initialize()
	{
		await Login();
		LoadJobs();
		UpdateJobCounts();
	}

	private async Task Login()
	{
		string? username = Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "CsabaBlazsek";
		string? password = Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? "csabi?3164";

		try
		{
			await _authService.AuthenticateAsync(username, password);
			LoadJobs();
			_authService.StartTokenRefreshTimer();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Login failed: {ex.Message}");
		}
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
			var filteredJobs = allJobs.Where(j => j.Location != null && j.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
				&& j.MeasurementDate.HasValue
				&& j.MeasurementDate.Value.Year == SelectedYear
				&& j.MeasurementDate.Value.Month == SelectedMonth).ToList();

			foreach (var job in filteredJobs)
			{
				job.PropertyChanged += (s, e) => HandleJobCompletionChanged(job);
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
													 && j.MeasurementDate.Value.Month == SelectedMonth).ToList();

		foreach (var job in jobsForSelectedDate)
		{
			job.PropertyChanged += (s, e) => HandleJobCompletionChanged(job);
			Jobs.Add(job);
		}
		UpdateJobCounts();
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

	private async void HandleJobCompletionChanged(Job job)
	{
		if (job.IsCompleted)
		{
			var result = MessageBox.Show("Are you sure you want to mark this job as completed?", "Confirm Completion", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.No)
			{
				job.IsCompleted = false;
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
				job.IsCompleted = true;
			}
		}
	}

	private void NavigateLeft()
	{
		if (_selectedMonth == 1)
		{
			if (_selectedYear > Years.Min())
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
			if (_selectedYear < Years.Max())
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
		MonthlyJobCount = Jobs.Count(j => j.MeasurementDate?.Year == SelectedYear && j.MeasurementDate?.Month == SelectedMonth);
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
