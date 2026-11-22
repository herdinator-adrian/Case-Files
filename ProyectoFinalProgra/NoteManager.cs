using System;

public class NoteManager
{
	public static readonly string notesPath = Path.Combine(DocumentManager.gamePath, "Notes/");

	public static void AddNote(string noteName, string message)
	{
		if (!noteName.EndsWith(".txt")) noteName += ".txt";

		string? directory = Path.GetDirectoryName(notesPath);
		if (directory != null)
		{
			Directory.CreateDirectory(directory);
		}

		string path = Path.Combine(notesPath, noteName);

		if (!File.Exists(path))
			Console.WriteLine($"New note created as {Path.GetRelativePath(notesPath, path)}");

		File.AppendAllText(path, message + Environment.NewLine);
	}

	public static void ReadNote(string noteName)
	{
		if (!noteName.EndsWith(".txt")) noteName += ".txt";

		string content = FileManager.ReadFile(Path.Combine(notesPath, noteName));
		if (content.IsWhiteSpace())
		{
			Console.WriteLine("[You haven't written anything in this note...]");
			return;
		}
		Console.WriteLine(content);
	}

	public static void UndoNote(string noteName)
	{
		if (!noteName.EndsWith(".txt")) noteName += ".txt";

		string path = Path.Combine(notesPath, noteName);
		if (!FileManager.FileExists(path)) return;

		string[] lines = File.ReadAllLines(path);

		if (lines.Length > 0)
		{
			File.WriteAllLines(path, lines[..^1]);
		}
	}

	public static void DeleteNote(string noteName)
	{
		if (!noteName.EndsWith(".txt")) noteName += ".txt";

		string path = Path.Combine(notesPath, noteName);
		if (!FileManager.FileExists(path)) return;

		FileManager.DeleteFile(path);
	}
}
