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
		window?.Show();
	}

	public void NavigateTo<T>(object parameter) where T : Window
	{
		var window = _serviceProvider.GetService(typeof(T)) as Window;
		if (window != null)
		{
			if (window.DataContext is IWindowParameterReceiver receiver)
			{
				receiver.ReceiveParameter(parameter);
			}
			window.Show();
		}
	}
}

public interface IWindowParameterReceiver
{
	void ReceiveParameter(object parameter);
}
