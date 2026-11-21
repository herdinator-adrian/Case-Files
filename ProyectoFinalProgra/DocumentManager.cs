using System;
using System.IO;
public class DocumentManager
{
	private static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;

	private static readonly string sourcePath = Path.Combine(basePath, "GameData");
	public static readonly string playerDataPath = Path.Combine(basePath, "PlayerData");
	public static readonly string gamePath = Path.Combine(playerDataPath, "Files");
	public static string GetPath(string path) => Path.Combine(gamePath, path);

	public static void InitializeGame()
	{
		Directory.CreateDirectory(playerDataPath);
		Directory.CreateDirectory(gamePath);
		CopyDirectoryStructure(sourcePath, gamePath);

		Directory.CreateDirectory(Path.Combine(playerDataPath, "System"));

		Directory.CreateDirectory(Path.Combine(playerDataPath, "Save"));
	}

	public static void UnlockDocument(string fileName)
	{
		string sourcePath = Path.Combine(DocumentManager.sourcePath, fileName);

		string destinationPath = Path.Combine(gamePath, fileName);

		if (!File.Exists(sourcePath))
		{
			Console.WriteLine("Source document not found.");
			return;
		}

		// Don't duplicate.
		if (File.Exists(destinationPath))
		{
			Console.WriteLine("Document already unlocked.");
			return;
		}

		File.Copy(sourcePath, destinationPath);

		NotificationManager.AddNotification("You have access to a new file: " + fileName);
	}

	#region HELPER
	
	static void CopyDirectoryStructure(string source, string destination)
	{
		Directory.CreateDirectory(destination);

		foreach (string directory in Directory.GetDirectories(source))
		{
			string directoryName = Path.GetFileName(directory);
			string newPath = Path.Combine(destination, directoryName);

			CopyDirectoryStructure(directory, newPath);
		}
	}
	#endregion
}