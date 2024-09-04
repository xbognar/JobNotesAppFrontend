using JobNotesWPF.Models;
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
using System.Windows.Shapes;

namespace JobNotesWPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

		private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (e.EditAction == DataGridEditAction.Commit)
			{
				var dataGrid = sender as DataGrid;
				var editedJob = dataGrid?.SelectedItem as Job;

				if (editedJob != null)
				{
					// Call the UpdateJob method from the ViewModel to update the job in the database
					var viewModel = (MainViewModel)DataContext;
					viewModel.UpdateJobCommand.Execute(editedJob);
				}
			}
		}


	}
}
