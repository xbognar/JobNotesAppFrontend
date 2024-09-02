using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JobNotesWPF.Models
{
	public class Job : INotifyPropertyChanged
	{
		private int _id;
		private int _serialNumber;
		private string? _jobNumber;
		private string? _location;
		private string? _clientName;
		private DateTime? _measurementDate;
		private string? _notes;
		private bool _isCompleted;

		public int Id
		{
			get => _id;
			set => SetProperty(ref _id, value);
		}

		public int SerialNumber
		{
			get => _serialNumber;
			set => SetProperty(ref _serialNumber, value);
		}

		public string? JobNumber
		{
			get => _jobNumber;
			set => SetProperty(ref _jobNumber, value);
		}

		public string? Location
		{
			get => _location;
			set => SetProperty(ref _location, value);
		}

		public string? ClientName
		{
			get => _clientName;
			set => SetProperty(ref _clientName, value);
		}

		public DateTime? MeasurementDate
		{
			get => _measurementDate;
			set => SetProperty(ref _measurementDate, value);
		}

		public string? Notes
		{
			get => _notes;
			set => SetProperty(ref _notes, value);
		}

		public bool IsCompleted
		{
			get => _isCompleted;
			set
			{
				if (SetProperty(ref _isCompleted, value))
				{
					// Trigger an update to disable editing if marked as completed
					OnPropertyChanged(nameof(IsEditable));
				}
			}
		}

		// Property to determine if the job can be edited
		public bool IsEditable => !_isCompleted;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
