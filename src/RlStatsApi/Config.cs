using System.Globalization;
using System.Net;
using FileIni;

namespace RlStatsApi;

public static class Config
{
	public const int PortDefault = 49123;
	public const int PortMin = 1;
	public const int PortMax = IPEndPoint.MaxPort;
	public const double PacketSendRateDefault = 0;
	public const double PacketSendRateDisabled = 0;
	public const double PacketSendRateMin = 0;
	public const double PacketSendRateMax = 120;

	private static readonly string ConfigFileRelativePath = Path.Combine(
		"TAGame",
		"Config",
		"DefaultStatsAPI.ini");
	private const string Section = "TAGame.MatchStatsExporter_TA";
	private const string PortKey = "Port";
	private const string PacketSendRateKey = "PacketSendRate";

	public static bool IsValid(int? port, double? packetSendRate, string rlInstallDir)
	{
		IniFile configFile = GetConfigFile(rlInstallDir);
		int normalizedPort = NormalizePort(port);
		double normalizedPacketSendRate = NormalizePacketSendRate(packetSendRate);

		try
		{
			return CheckConfigValues(normalizedPort, normalizedPacketSendRate, configFile);
		}
		catch (Exception exception)
			when (
				exception is KeyNotFoundException
				|| exception is FormatException
				|| exception is OverflowException
			)
		{
			return false;
		}
	}

	public static void Apply(int? port, double? packetSendRate, string rlInstallDir)
	{
		IniFile configFile = GetConfigFile(rlInstallDir);
		int normalizedPort = NormalizePort(port);
		double normalizedPacketSendRate = NormalizePacketSendRate(packetSendRate);

		configFile.Set(Section, PortKey, normalizedPort.ToString(CultureInfo.InvariantCulture));
		configFile.Set(Section, PacketSendRateKey, normalizedPacketSendRate.ToString(CultureInfo.InvariantCulture));
		configFile.Write();
	}

	private static IniFile GetConfigFile(string rlInstallDir)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rlInstallDir);
		string trimmedDir = rlInstallDir.Trim();
		if (!Directory.Exists(trimmedDir))
			throw new DirectoryNotFoundException($"Rocket League install dir does not exist: \"{trimmedDir}\".");
		
		string configFilePath = Path.Combine(trimmedDir, ConfigFileRelativePath);
		if (!File.Exists(configFilePath))
			throw new FileNotFoundException($"Rocket League API config file not found: \"{configFilePath}\".", configFilePath);
		
		IniFile configFile = new(configFilePath);
		configFile.Read();
		return configFile;
	}

	private static int NormalizePort(int? port)
	{
		if (port == null)
			return PortDefault;
		if (port < PortMin || port > PortMax)
			throw new ArgumentOutOfRangeException(
				nameof(port),
				port,
				$"Invalid port {port}: must be between {PortMin} and {PortMax} (inclusive)."
			);
		return port.Value;
	}

	private static double NormalizePacketSendRate(double? packetSendRate)
	{
		if (packetSendRate == null)
			return PacketSendRateDefault;
		if (!double.IsFinite(packetSendRate.Value))
			throw new ArgumentOutOfRangeException(
				nameof(packetSendRate),
				packetSendRate,
				$"Invalid packetSendRate {packetSendRate}: must be finite.");
		if (packetSendRate < PacketSendRateMin || packetSendRate > PacketSendRateMax)
			throw new ArgumentOutOfRangeException(
				nameof(packetSendRate),
				packetSendRate,
				$"Invalid packetSendRate {packetSendRate}: must be between {PacketSendRateMin} and {PacketSendRateMax} (inclusive)."
			);
		return packetSendRate.Value;
	}

	private static bool CheckConfigValues(int port, double packetSendRate, IniFile configFile)
	{
		string portString = configFile.Get(Section, PortKey);
		int currentPort = int.Parse(portString, CultureInfo.InvariantCulture);
		if (currentPort != port)
			return false;
		
		string packetSendRateString = configFile.Get(Section, PacketSendRateKey);
		double currentPacketSendRate = double.Parse(packetSendRateString, CultureInfo.InvariantCulture);
		if (currentPacketSendRate != packetSendRate)
			return false;
		
		return true;
	}
}
