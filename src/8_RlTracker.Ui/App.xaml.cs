using System.Windows;
using GuillaumeAst.RlTracker.Core;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui;

public partial class App : Application
{
	protected override async void OnStartup(StartupEventArgs eventArgs)
	{
		base.OnStartup(eventArgs);

		try
		{
			await Driver.Instance.Start();
			MainWindow window = new();
			window.Show();
		}
		catch (Exception exception)
		{
			Log.PrintRed($"❌ ERROR: {exception.Message}");
			Shutdown(1);
		}
	}

	protected override async void OnExit(ExitEventArgs eventArgs)
	{
		await Driver.Stop();
		base.OnExit(eventArgs);
	}
}
