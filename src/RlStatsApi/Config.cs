using System.Text.Json.Serialization;
using System.Globalization;
using System.Net;
using FileIni;

namespace RlStatsApi;

public sealed class Config(int? port, double? packetSendRate)
{
	public static readonly string ConfigFileRelativePath = Path.Combine(
		"TAGame",
		"Config",
		"DefaultStatsAPI.ini");
	public const int PortDefault = 49123;
	public const int PortMin = 1;
	public const int PortMax = IPEndPoint.MaxPort;
	public const double PacketSendRateDefault = 0;
	public const double PacketSendRateDisabled = 0;
	public const double PacketSendRateMin = 0;
	public const double PacketSendRateMax = 120;

	private const string Section = "TAGame.MatchStatsExporter_TA";
	private const string PortKey = "Port";
	private const string PacketSendRateKey = "PacketSendRate";

	public int Port { get; } = NormalizePort(port);
	public double PacketSendRate { get; } = NormalizePacketSendRate(packetSendRate);

	[JsonConstructor]
	public Config(int port, double packetSendRate)
		: this((int?)port, (double?)packetSendRate)
	{
	}

	public void Apply(string rlInstallDir, ref bool rlNeedRestart)
	{
		IniFile configFile = GetConfigFile(rlInstallDir);

		if (IsApplied(configFile))
			return;
		configFile.Set(Section, PortKey, Port.ToString(CultureInfo.InvariantCulture));
		configFile.Set(Section, PacketSendRateKey, PacketSendRate.ToString(CultureInfo.InvariantCulture));
		configFile.Write();
		rlNeedRestart = true;
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
				$"Invalid packet send rate {packetSendRate}: must be finite.");
		if (packetSendRate < PacketSendRateMin || packetSendRate > PacketSendRateMax)
			throw new ArgumentOutOfRangeException(
				nameof(packetSendRate),
				packetSendRate,
				$"Invalid packet send rate {packetSendRate}: must be between {PacketSendRateMin} and {PacketSendRateMax} (inclusive)."
			);
		return packetSendRate.Value;
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

	private bool IsApplied(IniFile configFile)
	{
		try
		{
			return CheckConfigValues(configFile);
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

	private bool CheckConfigValues(IniFile configFile)
	{
		string portString = configFile.Get(Section, PortKey);
		int currentPort = int.Parse(portString, CultureInfo.InvariantCulture);
		if (currentPort != Port)
			return false;
		
		string packetSendRateString = configFile.Get(Section, PacketSendRateKey);
		double currentPacketSendRate = double.Parse(packetSendRateString, CultureInfo.InvariantCulture);
		if (currentPacketSendRate != PacketSendRate)
			return false;
		
		return true;
	}
}
