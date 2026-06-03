namespace GuillaumeAst.Utils;

public static class App
{
    public const string AppName = "RlTracker";
    public static readonly string AppDir = GetAppDir();

    private static string GetAppDir()
    {
		string? appDir = null;
		try
		{
			appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		catch (PlatformNotSupportedException)
		{
			// Fallback available
		}
		if (string.IsNullOrWhiteSpace(appDir))
		{
			try
			{
				appDir = Environment.GetEnvironmentVariable("HOME");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(appDir))
		{
			try
			{
				appDir = Environment.GetEnvironmentVariable("USERPROFILE");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(appDir))
		{
			return Path.Combine(AppContext.BaseDirectory, AppName);
		}
		return Path.Combine(appDir, AppName);
	}
}