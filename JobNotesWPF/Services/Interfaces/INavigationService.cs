using System.Windows;

public interface INavigationService
{

	void NavigateTo<T>() where T : Window;

}
