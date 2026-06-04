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

		Log.Init();
		if (eventArgs.Args.Contains("--debug"))
		{
			Log.LevelMin = Log.Level.Debug;
		}
		try
		{
			await Driver.Instance.Start();
			MainWindow window = new();
			window.Show();
			Log.Write(Log.Level.Warning, $"================> RlNotFound = {Driver.Instance.RlNotFound}");
			Log.Write(Log.Level.Warning, $"================> RlNeedRestart = {Driver.Instance.RlNeedRestart}");
		}
		catch (Exception exception)
		{
			Log.Write(Log.Level.Error, $"❌ ERROR: {exception.Message}:\n{exception}");
			Shutdown(_exitFailure);
		}
	}

	protected override async void OnExit(ExitEventArgs eventArgs)
	{
		await Driver.Instance.Stop();
		base.OnExit(eventArgs);
	}
}
