using JobNotesWPF.ViewModels;
using JobNotesWPF;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Navigation;
using JobNotesWPF.Views;

public partial class App : Application
{
	public IServiceProvider ServiceProvider { get; private set; }

	public App()
	{
		var serviceCollection = new ServiceCollection();
		ConfigureServices(serviceCollection);
		ServiceProvider = serviceCollection.BuildServiceProvider();
	}

	private void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton(new HttpClient { BaseAddress = new Uri("https://yourapiurl.com/") });

		services.AddTransient<IAuthenticationService, AuthenticationService>();
		services.AddTransient<IJobService, JobService>();
		services.AddTransient<INavigationService, NavigationService>();

		services.AddSingleton<MainViewModel>();
		services.AddSingleton<JobListViewModel>();

		services.AddSingleton<MainWindow>();
		services.AddSingleton<JobListWindow>();

	}

	protected override void OnStartup(StartupEventArgs e)
	{
		var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
		mainWindow.Show();

		base.OnStartup(e);
	}
}
