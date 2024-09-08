using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JobNotesWPF.Views;
using System.Windows.Shapes;
using JobNotesWPF.ViewModels;

namespace JobNotesWPF.Views
{
	/// <summary>
	/// Interaction logic for JobListWindow.xaml
	/// </summary>
	public partial class JobListWindow : Window
	{
		public JobListWindow(JobListViewModel jobListViewModel)
		{
			InitializeComponent();
			DataContext = jobListViewModel;
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (DataContext is JobListViewModel viewModel)
			{
				viewModel.LoadData(); 
			}
		}

	}
}
