using System;
using System.IO;
public class DocumentManager
{
	public static DateTime StoryDate { get; private set; } = DateTime.Now.Date;
	private const string storyDateStart = "1998/11/16";

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

		// SET AND SAVE/LOAD TIME
		DateTime storyStart = DateTime.Parse(storyDateStart);
		DateTime realStart = DateTime.Now.Date;
		string savePath = Path.Combine(playerDataPath, "Save", "date.txt");
		if (File.Exists(savePath))
		{
			realStart = DateTime.Parse(FileManager.ReadFile(savePath));
		}
		else
		{
			FileManager.CreateFile(savePath, realStart.ToString("dd/MM/yyyy"));
		}

		DateTime realNow = DateTime.Now;

		TimeSpan elapsed = realNow.Date - realStart.Date;

		StoryDate = storyStart.AddDays(elapsed.Days);
	}

	public static void ResetGame()
	{
		Formatter.WriteHeader("""
			You're about to reset the game, which will delete all your progress.
			This will also close the program, so if you want to try from the start, you will have to run it again.
			Are you sure you want to do this?
			""");
		if (Formatter.WriteYesNoQuestion(""))
			Directory.Delete(playerDataPath, true);

		Commands.Exit();
	}

	private static readonly string readFilesPath = Path.Combine(playerDataPath, "Save", "read_files.txt");

	/// <summary>
	/// Checks if a file has been marked as read.
	/// </summary>
	/// <param name="relativeFilePath"></param>
	/// <returns></returns>
	public static bool IsFileRead(string filePath)
	{
		if (!File.Exists(readFilesPath))
			return false;

		if (!FileManager.ReadFile(readFilesPath).Contains(Path.GetRelativePath(gamePath, filePath)))
			return true;
		return false;
	}
		

	/// <summary>
	/// Marks a file as read, which is useful to mantain an eye on progress of the story.
	/// </summary>
	/// <param name="filePath"></param>
	public static void MarkFileAsRead(string filePath)
	{
		filePath = Path.GetRelativePath(gamePath, filePath);

		if (IsFileRead(filePath)) return;

		if (!File.Exists(readFilesPath))
			FileManager.CreateFile(readFilesPath, filePath);
		else
			FileManager.AddToFile(readFilesPath, filePath);

		NotificationManager.AddNotification($"The file {filePath} was marked as read.");
	}

	/// <summary>
	/// Gets all the files that have been marked as read.
	/// </summary>
	/// <returns></returns>
	public static string[] GetReadFiles()
	{
		if (!File.Exists(readFilesPath)) return new string[]{ };
		
		string[] lines = File.ReadAllLines(readFilesPath);

		return lines;
	}

	/// <summary>
	/// Unlocks a file, copying the file from the original game data to the user data space.
	/// </summary>
	/// <param name="path"></param>
	public static void UnlockFile(string path)
	{
		string sourcePath = Path.Combine(DocumentManager.sourcePath, path);

		string destinationPath = Path.Combine(gamePath, path);

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

		NotificationManager.AddNotification("You have access to a new file: " + path);
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