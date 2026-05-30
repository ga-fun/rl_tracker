using System.Diagnostics;
using RlStatsApi;

namespace RlTracker.Core;

internal static class RlProcess
{
	private const string RlProcessName = "RocketLeague";

	internal static bool IsRunning()
	{
		Process[] processes = Process.GetProcessesByName(RlProcessName);
		bool isRunning = processes.Length > 0;

		foreach (Process process in processes)
			process.Dispose();
		return isRunning;
	}
}
