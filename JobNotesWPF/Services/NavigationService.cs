using System;
using System.Windows;

public class NavigationService : INavigationService
{
	private readonly IServiceProvider _serviceProvider;

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public void NavigateTo<T>() where T : Window
	{
		var window = _serviceProvider.GetService(typeof(T)) as Window;

		if (window != null)
		{
			window.Closed += (sender, args) =>
			{
				window = null; 
			};

			window.Show();
		}
	}


}
