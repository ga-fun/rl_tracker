using System.Diagnostics;

namespace GuillaumeAst.RocketLeague;

public static class RlProcess
{
	private const string ProcessName = "RocketLeague";

	public static bool IsRunning()
	{
		Process[] processes = Process.GetProcessesByName(ProcessName);
		bool isRunning = processes.Length > 0;

		foreach (Process process in processes)
		{
			process.Dispose();
		}
		return isRunning;
	}
}
