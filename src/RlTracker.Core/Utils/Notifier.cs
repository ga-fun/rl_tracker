using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RlTracker.Core;

public abstract class Notifier : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyChange([CallerMemberName] string? propertyName = null)
	{
		ArgumentNullException.ThrowIfNull(propertyName);
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
