using RlTracker.Core;

namespace RlTracker.Cli;

internal static class Program
{
	private const int ExitSuccess = 0;
	private const int ExitFailure = 1;
 
	private static int Main(string[] args)
	{
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		Console.WriteLine($"{Log.Blue}[RlTracker.Cli.Program.Main()]{Log.Reset}");
		Config config;

		config = Config.Load();
		Console.WriteLine($"Test emojis: {config.WpfConfig.WinPrefix} | {config.WpfConfig.LossPrefix} | {config.WpfConfig.WinStreakPrefix} | {config.WpfConfig.LossStreakPrefix}.");

		return (ExitSuccess);
	}
}
