using System;

public class FileManager
{
	public static void CreateFile(string path, string content = "")
	{
		// Crear el archivo.
		StreamWriter file = File.CreateText(path);
		file.WriteLine(content);
		//Console.WriteLine($"Creating file at {path}...");
		file.Close();
	}

	public static void AddToFile(string path, string content)
	{
		if (!FileExists(path)) return;
		StreamWriter add = File.AppendText(path);
		add.WriteLine(content);
		add.Close();
	}

	public static string ReadFile(string path)
	{
		if (!FileExists(path)) return "";
		// Leer el archivo.
		StreamReader reader = File.OpenText(path);
		string text = reader.ReadToEnd();
		reader.Close();
		return text;
	}

	public static void DeleteFile(string pathToDelete)
	{
		if (pathToDelete == DocumentManager.gamePath || !File.Exists(pathToDelete))
		{
			Console.WriteLine($"File in {pathToDelete} doesn't exists.");
			return;
		}

		File.Delete(pathToDelete);
	}

	public static bool FileExists(string path)
	{
		if (!File.Exists(path))
		{
			Console.WriteLine($"The file in the path \"{Path.GetRelativePath(DocumentManager.gamePath, path)}\" doesn't exists.");
			return false;
		}
		return true;
	}
}
