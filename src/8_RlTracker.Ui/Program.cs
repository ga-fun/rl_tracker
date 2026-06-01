using GuillaumeAst.RlTracker.Core;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui;

internal static class Program
{
	private const int _exitSuccess = 0;
	private const int _exitFailure = 1;

	private static async Task<int> Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Driver driver = Driver.Instance;
		CancellationTokenSource source = new();

		Console.CancelKeyPress += (_, eventArgs) =>
		{
			eventArgs.Cancel = true;
			source.Cancel();
		};

		try
		{
			await driver.Start();
			await Task.Delay(Timeout.Infinite, source.Token);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception exception)
		{
			Log.PrintRed($"❌ ERROR: {exception.Message}.");
			return _exitFailure;
		}
		finally
		{
			await driver.Stop();
		}
		return _exitSuccess;
	}
}
