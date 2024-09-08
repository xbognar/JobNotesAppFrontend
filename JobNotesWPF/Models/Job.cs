using System.ComponentModel;

namespace DataAccess.Models
{
	public class Job : INotifyPropertyChanged
	{
		private bool _isCompleted;

		public int Id { get; set; }
		public int SerialNumber { get; set; }
		public string? JobNumber { get; set; }
		public string? Location { get; set; }
		public string? ClientName { get; set; }
		public DateTime? MeasurementDate { get; set; }
		public string? Notes { get; set; }

		public bool IsCompleted
		{
			get => _isCompleted;
			set
			{
				if (_isCompleted != value)
				{
					_isCompleted = value;
					OnPropertyChanged(nameof(IsCompleted));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
