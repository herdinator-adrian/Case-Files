using System;

public class NotificationManager
{
	private static readonly string notificationsPath = Path.Combine(DocumentManager.playerDataPath, "notifications.txt");

	public static void AddNotification(string message)
	{
		string time = DateTime.Now.ToString("HH:mm");

		string notification =
			$"[{time}, {DocumentManager.StoryDate.Date.ToString("dd/MM/yyyy")}] {message}";

		Console.WriteLine(notification);

		string? directory = Path.GetDirectoryName(notificationsPath);
		if (directory != null)
		{
			Directory.CreateDirectory(directory);
		}

		File.AppendAllText(notificationsPath, notification + Environment.NewLine);
	}

	public static void ShowNotifications()
	{
		if (!File.Exists(notificationsPath))
		{
			Console.WriteLine("There are currently no logs.");
			return;
		}

		Console.WriteLine(FileManager.ReadFile(notificationsPath));
	}
}
