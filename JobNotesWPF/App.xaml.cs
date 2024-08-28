using JobNotesWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;
using dotenv.net;
using JobNotesWPF.Views;

public partial class App : Application
{
	public IServiceProvider ServiceProvider { get; private set; }

	public App()
	{
		DotEnv.Load();

		var serviceCollection = new ServiceCollection();
		ConfigureServices(serviceCollection);
		ServiceProvider = serviceCollection.BuildServiceProvider();
	}

	private void ConfigureServices(IServiceCollection services)
	{
		var apiBaseUrl = "http://localhost:5000/";

		services.AddSingleton(new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
		services.AddTransient<IAuthenticationService, AuthenticationService>();
		services.AddTransient<IJobService, JobService>();
		services.AddTransient<INavigationService, NavigationService>();
		services.AddSingleton<MainViewModel>();
		services.AddSingleton<JobListViewModel>();
		services.AddSingleton<MainWindow>();
		services.AddSingleton<JobListWindow>();
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
		await mainViewModel.Initialize();

		var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
		mainWindow.Show();

		base.OnStartup(e);
	}
}
