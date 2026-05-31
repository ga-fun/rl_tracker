using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GuillaumeAst.Utils;

public abstract class Notifier : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyChange([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
