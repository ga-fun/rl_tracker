using System.Windows;
using GuillaumeAst.RlTracker.Core;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui;

public partial class App : Application
{
	private const int _exitFailure = 1;

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
			Log.Write(Log.Level.Error, $"❌ ERROR: {exception.Message}");
			Shutdown(_exitFailure);
		}
	}

	protected override async void OnExit(ExitEventArgs eventArgs)
	{
		await Driver.Instance.Stop();
		base.OnExit(eventArgs);
	}
}
