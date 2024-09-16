using JobNotesWPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;
using JobNotesWPF.Views;

namespace JobNotesWPF
{
	public partial class App : Application
	{
		public IServiceProvider ServiceProvider { get; private set; }

		public App()
		{
			DotNetEnv.Env.Load();
			DotNetEnv.Env.TraversePath().Load();

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
			services.AddTransient<JobListViewModel>();
			services.AddSingleton<MainWindow>();
			services.AddTransient<JobListWindow>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			var username = System.Environment.GetEnvironmentVariable("USERNAME");
			var password = System.Environment.GetEnvironmentVariable("PASSWORD");

			var authService = ServiceProvider.GetRequiredService<IAuthenticationService>();
			var token = await authService.AuthenticateAsync(username, password);

			if (!string.IsNullOrEmpty(token))
			{
				var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
				mainViewModel.Initialize();

				var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
				mainWindow.Show();
			}
			else
			{
				MessageBox.Show("Authentication failed. The application will now exit.");
				Shutdown();
			}

			base.OnStartup(e);
		}
	}
}
