using System.Text.Json.Serialization;
using System.Globalization;
using System.Net;
using GuillaumeAst.FileIni;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class Config : Notifier
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

	[JsonConstructor]
	public Config(int port, double packetSendRate)
		: this((int?)port, (double?)packetSendRate)
	{
	}

	public Config(int? port, double? packetSendRate)
	{
		Port = NormalizePort(port);
		PacketSendRate = NormalizePacketSendRate(packetSendRate);
	}

	public int Port
	{
		get;
		set
		{
			int normalized = NormalizePort(value);
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}
	}
	public double PacketSendRate
	{
		get;
		set
		{
			double normalized = NormalizePacketSendRate(value);
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}	
	}

	public void Apply(string rlInstallDir, ref bool rlNeedRestart)
	{
		IniFile configFile = LoadConfigFile(rlInstallDir);

		if (IsApplied(configFile))
		{
			return;
		}
		configFile.Set(Section, PortKey, Port.ToString(CultureInfo.InvariantCulture));
		configFile.Set(Section, PacketSendRateKey, PacketSendRate.ToString(CultureInfo.InvariantCulture));
		configFile.Write();
		rlNeedRestart = true;
	}

	private static int NormalizePort(int? port)
	{
		if (port == null)
		{
			return PortDefault;
		}
		if (port < PortMin || port > PortMax)
		{
			throw new ArgumentOutOfRangeException(
				nameof(port),
				port,
				$"Invalid port {port}: must be between {PortMin} and {PortMax} (inclusive)."
			);
		}
		return port.Value;
	}

	private static double NormalizePacketSendRate(double? packetSendRate)
	{
		if (packetSendRate == null)
		{
			return PacketSendRateDefault;
		}
		if (!double.IsFinite(packetSendRate.Value))
		{
			throw new ArgumentOutOfRangeException(
				nameof(packetSendRate),
				packetSendRate,
				$"Invalid packet send rate {packetSendRate}: must be finite.");
		}
		if (packetSendRate < PacketSendRateMin || packetSendRate > PacketSendRateMax)
		{
			throw new ArgumentOutOfRangeException(
				nameof(packetSendRate),
				packetSendRate,
				$"Invalid packet send rate {packetSendRate}: must be between {PacketSendRateMin} and {PacketSendRateMax} (inclusive)."
			);
		}
		return packetSendRate.Value;
	}

	private static IniFile LoadConfigFile(string rlInstallDir)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(rlInstallDir);
		string trimmedDir = rlInstallDir.Trim();
		if (!Directory.Exists(trimmedDir))
		{
			throw new DirectoryNotFoundException($"Rocket League install dir does not exist: \"{trimmedDir}\".");
		}
		
		string configFilePath = Path.Combine(trimmedDir, ConfigFileRelativePath);
		if (!File.Exists(configFilePath))
		{
			throw new FileNotFoundException($"Rocket League API config file not found: \"{configFilePath}\".", configFilePath);
		}		
		return IniFile.Read(configFilePath);
	}

	private bool IsApplied(IniFile configFile)
	{
		string portString;
		string psrString;
		int currentPort;
		double currentPSR;

		try
		{
			portString = configFile.Get(Section, PortKey);
			currentPort = int.Parse(portString, CultureInfo.InvariantCulture);
			psrString = configFile.Get(Section, PacketSendRateKey);
			currentPSR = double.Parse(psrString, CultureInfo.InvariantCulture);
		}
		catch (Exception exception) when (exception
			is KeyNotFoundException
			or FormatException
			or OverflowException)
		{
			return false;
		}
		return currentPort == Port && currentPSR == PacketSendRate;
	}
}
