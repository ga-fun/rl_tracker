using System.Windows;
using System.Diagnostics;
using GuillaumeAst.RlTracker.Settings;
using GuillaumeAst.RlTracker.Ui.ViewModels;

namespace GuillaumeAst.RlTracker.Ui;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainWindowViewModel();
	}

	private void OpenConfigButton_Click(object sender, RoutedEventArgs eventArgs)
	{
		Process.Start(new ProcessStartInfo
		{
			FileName = Config.ConfigFile,
			UseShellExecute = true
		});
	}
}
