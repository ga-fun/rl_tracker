using System.Windows.Controls;
using GuillaumeAst.RlTracker.Ui.ViewModels;

namespace GuillaumeAst.RlTracker.Ui.Views;

public partial class MainTrackerView : UserControl
{
	public MainTrackerView()
	{
		InitializeComponent();
		DataContext = new MainTrackerViewModel();
	}
}
