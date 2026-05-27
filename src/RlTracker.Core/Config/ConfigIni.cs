using System.Globalization;

namespace RlTracker.Core.Config;

internal sealed class ConfigIni
{


	public string RlDir { get; }
	public string RlIniFile { get; }

	public ConfigIni(string rlDir)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rlDir);

		RlDir = rlDir;
		RlIniFile = Path.Combine(RlDir, "TAGame", "Config", "DefaultStatsAPI.ini");
	}

	public bool IsReady(int port, double packetSendRate)
	{
		Dictionary<string, string> values;
		int currentPort;
		double currentPacketSendRate;

		if (!File.Exists(RlIniFile))
			throw new FileNotFoundException($"Rocket League config file not found: \"{RlIniFile}\".");
		values = ReadSectionValues();
		currentPort = ;
		currentPacketSendRate = ;
		return port == currentPort && packetSendRate == currentPacketSendRate;
	}

	public bool Init(int port, double packetSendRate)
	{
		// TODO: update port and packetSendRate inside RlIniFile
	}
}
