using DataAccess.Models;
using JobNotesWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Specialized;

namespace JobNotesWPF.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow(MainViewModel mainViewModel)
		{
			InitializeComponent();
			DataContext = mainViewModel;

			mainViewModel.Jobs.CollectionChanged += Jobs_CollectionChanged;
		}

		private void Jobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Application.Current.Dispatcher.InvokeAsync(() =>
			{
				foreach (var job in JobDataGrid.Items)
				{
					var row = JobDataGrid.ItemContainerGenerator.ContainerFromItem(job) as DataGridRow;
					if (row != null)
					{
						var jobItem = job as Job;
						if (jobItem != null)
						{
							if (jobItem.IsCompleted)
							{
								row.Background = (Brush)FindResource("StripedPatternForCompletedRows");
							}
							else
							{
								row.Background = (Brush)FindResource("UncompletedJobBackgroundBrush");
							}
						}
					}
				}
			}, System.Windows.Threading.DispatcherPriority.Loaded);
		}


		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			var job = button?.DataContext as Job;

			if (job != null)
			{
				if (JobDataGrid.CommitEdit(DataGridEditingUnit.Row, true))
				{
					var viewModel = this.DataContext as MainViewModel;
					viewModel?.SaveJobCommand.Execute(job);

					var row = JobDataGrid.ItemContainerGenerator.ContainerFromItem(job) as DataGridRow;
					if (row != null)
					{
						if (job.IsCompleted)
						{
							row.Background = (Brush)FindResource("StripedPatternForCompletedRows");
						}
						else
						{
							row.Background = (Brush)FindResource("UncompletedJobBackgroundBrush");
						}
					}
				}
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			JobDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
		}

	}
}
