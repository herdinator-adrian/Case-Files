using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class Commands
{
	public static void HandleCommand(string command, string[] args)
	{
		switch (command)
		{
			case "unlock":
				Unlock(args);
				break;
			case "open":
				Open(args);
				break;
			case "list":
				List();
				break;
			case "search":
				Search(args);
				break;
			case "log":
				Log();
				break;
			case "help":
				Help();
				break;

			case "add_note":
				AddNote(args);
				break;
			case "read_note":
				ReadNote(args);
				break;
			case "undo_note":
				UndoNote(args);
				break;
			case "delete_note":
				DeleteNote(args);
				break;

			case "exit":
				Exit();
				break;

			default:
				Console.WriteLine($"\"{command}\" is not a command.");
				break;
		}
	}

	/*
	public static void Create(string[] args)
	{
		if (args.Length < 1 || args.Length > 2)
		{
			Console.WriteLine("The command 'create' should at least have one argument: name, content (optional).");
			return;
		}

		string newPath = $"{DocumentManager.gamePath}/{args[0]}";

		if (File.Exists(newPath))
		{
			Console.WriteLine("The file already exists. If you wish to override the file, use the command \"override\"");
			return;
		}

		
		if (args.Length == 1)
			FileManager.CreateFile(newPath);
		else if (args.Length == 2)
			FileManager.CreateFile(newPath, args[1]);
	}

	public static void Override(string[] args)
	{
		if (args.Length != 2)
		{
			Console.WriteLine("The command 'override' should have two arguments: name, content");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);
		if (!File.Exists(newPath))
		{
			Console.WriteLine("The file doesn't exists. If you wish to create the file, use the command \"create\"");
			return;
		}
		FileManager.CreateFile(newPath, args[1]);
	}*/

	public static void Unlock(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'unlock' should at least have one argument: file name.");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);

		DocumentManager.UnlockDocument(args[0]);
	}

	public static void AddNote(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine("The command 'add_note' should have two arguments: noteName, content.");
			return;
		}

		string[] addArgs = { args[0], string.Join(" ", args[1..]) };

		NoteManager.AddNote(addArgs[0], addArgs[1]);
	}

	public static void ReadNote(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'read_note' should have one arguments: noteName.");
			return;
		}

		NoteManager.ReadNote(args[0]);
	}

	public static void UndoNote(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'undo_note' should have one argument: noteName");
			return;
		}

		NoteManager.UndoNote(args[0]);
	}
	
	public static void DeleteNote(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'delete_note' should have one argument: noteName");
			return;
		}

		Console.WriteLine("Are you sure you want to delete this file? (Y/n)");
		Console.Write("> ");
		string answer = Console.ReadLine().Trim().ToLower();
		if (answer == "y") NoteManager.DeleteNote(args[0]);
	}
	
	public static void Open(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'open' should have two arguments: file name");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);
		if (!FileManager.FileExists(newPath)) return;

		Formatter.ProcessText(FileManager.ReadFile(newPath));
	}

	public static void List()
	{
		Console.WriteLine();
		ListRecursive(DocumentManager.gamePath, 0);
		Console.WriteLine();

		void ListRecursive(string currentPath, int recursions)
		{
			string spaces = new string(' ', recursions * 2);

			string[] directories = Directory.GetDirectories(currentPath);
			foreach (string directory in directories)
			{
				Console.WriteLine($"  {spaces}|- {Path.GetRelativePath(currentPath, directory)}/");
				ListRecursive(Path.Combine(currentPath, directory), recursions + 1);
			}
			
			string[] files = Directory.GetFiles(currentPath, "*.txt", SearchOption.TopDirectoryOnly);
			foreach (string file in files)
			{
				Console.WriteLine($"  {spaces}|- {Path.GetRelativePath(currentPath, file)}");
			}
		}
	}

	public static void Search(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'search' should have one argument: keywords");
			return;
		}

		int charsToShowAfterAndBefore = 20;

		Console.WriteLine();
		SearchRecursive(DocumentManager.gamePath, 0);
		Console.WriteLine();

		void SearchRecursive(string currentPath, int recursions)
		{
			string[] directories = Directory.GetDirectories(currentPath);
			foreach (string directory in directories)
			{
				SearchRecursive(Path.Combine(currentPath, directory), recursions + 1);
			}

			string[] files = Directory.GetFiles(currentPath, "*.txt", SearchOption.TopDirectoryOnly);
			foreach (string file in files)
			{
				string fileContent = FileManager.ReadFile(file);

				MatchCollection matches = Regex.Matches(
					fileContent,
					@"\b" + Regex.Escape(args[0]) + @"\b",
					RegexOptions.IgnoreCase
				);

				if (matches.Count > 0)
				{
					string path = $"  |- {Path.GetRelativePath(DocumentManager.gamePath, file)}";
					string spaces = new string('-', 60 - path.Length);

					Formatter.WriteLine($"{path} {spaces}> {matches.Count} matches found.");
					Console.WriteLine();

					foreach (Match match in matches)
					{
						// Get the text before the match
						int start = Math.Max(0, match.Index - charsToShowAfterAndBefore);
						string before = fileContent.Substring(start, match.Index - start);

						// Keep only the last line
						before = before.Substring(before.LastIndexOf('\n') + 1);

						// Get the text after the match
						int end = Math.Min(fileContent.Length, match.Index + match.Length + charsToShowAfterAndBefore);
						string after = fileContent.Substring(match.Index + match.Length, end - (match.Index + match.Length));

						// Keep only the first line
						int nextLine = after.IndexOf('\n');
						if (nextLine != -1)
							after = after.Substring(0, nextLine);

						Console.WriteLine($"    |- {before}{match.Value.ToUpper()}{after}");
					}
				}
			}
		}
	}

	public static void Notify(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'notify' should have one argument: message");
			return;
		}

		NotificationManager.AddNotification(args[0]);
	}

	public static void Log()
	{
		NotificationManager.ShowNotifications();
	}

	public static void Help()
	{
		Formatter.WriteLine("\n--- Help ---");
		Console.WriteLine();
		Console.WriteLine("open - Opens a file. Args: fileName");
		Console.WriteLine("list - Lists the files saved.");
		Console.WriteLine("search - Searches for a word in all the files and shows number of matches and coincidences.");
		Console.WriteLine("log - Shows the log of your previous actions and the documents you've unlocked.");
		Formatter.DrawLine();
		Console.WriteLine();
		Console.WriteLine("add_note - Creates a file/adds content to an existing file. Args: noteName, content");
		Console.WriteLine("read_note - Reads an existing note. Args: noteName");
		Console.WriteLine("undo_note - Erases the last line written of an existing note. Args: noteName");
		Console.WriteLine("delete_note - Deletes an existing note. Args: noteName");
		Formatter.DrawLine();
		Console.WriteLine();
		Console.WriteLine("help - Shows this help menu.");
		Console.WriteLine("exit - Exit the program.");
	}

	public static void Exit() => Environment.Exit(0);
}
