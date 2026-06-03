namespace GuillaumeAst.Utils;

public static class App
{
    public const string AppName = "RlTracker";
    public static readonly string Directory = GetDirectory();

    private static string GetDirectory()
    {
		string? appDirectory = null;
		try
		{
			appDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		catch (PlatformNotSupportedException)
		{
			// Fallback available
		}
		if (string.IsNullOrWhiteSpace(appDirectory))
		{
			try
			{
				appDirectory = Environment.GetEnvironmentVariable("HOME");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(appDirectory))
		{
			try
			{
				appDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
			}
			catch (System.Security.SecurityException)
			{
				// Fallback available
			}
		}
		if (string.IsNullOrWhiteSpace(appDirectory))
		{
			return Path.Combine(AppContext.BaseDirectory, AppName);
		}
		return Path.Combine(appDirectory, AppName);
	}
}