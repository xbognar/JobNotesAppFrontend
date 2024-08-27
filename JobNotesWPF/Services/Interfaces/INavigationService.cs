using System.Windows;

public interface INavigationService
{

	void NavigateTo<T>() where T : Window;
	
	void NavigateTo<T>(object parameter) where T : Window;

}
