using System.Windows;
using GuillaumeAst.RlTracker.Ui.ViewModels;

namespace GuillaumeAst.RlTracker.Ui;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainTrackerViewModel();
	}
}
