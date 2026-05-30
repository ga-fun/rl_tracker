using RlTracker.Core;

namespace RlTracker.Cli;

internal static class Program
{
	private const int _exitSuccess = 0;
	private const int _exitFailure = 1;

	private static async Task<int> Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Driver driver = Driver.Instance;

		Console.CancelKeyPress += async (_, eventArgs) =>
		{
			eventArgs.Cancel = true;
			await driver.Stop();
		};

		try
		{
			await driver.Start();
		}
		catch (Exception exception)
		{
			Console.WriteLine($"${Log.Red}❌ ERROR: {Log.Yellow}{exception.Message}{Log.Reset}");
			return _exitFailure;
		}
		return _exitSuccess;
	}
}
