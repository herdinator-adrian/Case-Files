using System;

public static class Commands
{
	public static void HandleCommand(string command, string[] args)
	{
		switch (command)
		{
			case "create":
				Create(args);
				break;
			case "unlock":
				Unlock(args);
				break;
			case "add":
				Add(args);
				break;
			case "read":
				Read(args);
				break;
			case "override":
				Override(args);
				break;
			case "open":
				Open(args);
				break;
			case "delete":
				Delete(args);
				break;
			case "list":
				List();
				break;
			case "log":
				Log();
				break;
			case "help":
				Help();
				break;
			case "exit":
				Exit();
				break;
			default:
				Console.WriteLine($"\"{command}\" is not a command.");
				break;
		}
	}

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

	public static void Add(string[] args)
	{
		if (args.Length != 2)
		{
			Console.WriteLine("The command 'add' should have two arguments: name, content.");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);
		if (!FileManager.FileExists(newPath)) return;

		FileManager.AddToFile(newPath, args[1]);
	}

	public static void Read(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'read' should have one arguments: name.");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);
		if (!FileManager.FileExists(newPath)) return;

		Console.WriteLine(FileManager.ReadFile(newPath));
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

	public static void Delete(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("The command 'delete' should have one argument: name");
			return;
		}

		string newPath = DocumentManager.GetPath(args[0]);
		if (!File.Exists(newPath))
		{
			Console.WriteLine("The file doesn't exists");
			return;
		}

		Console.WriteLine("Are you sure you want to delete this file? (Y/n)");
		Console.Write("> ");
		string answer = Console.ReadLine().Trim().ToLower();
		if (answer == "y") FileManager.DeleteFile(newPath);
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
		Console.WriteLine("\n--- Help ---");
		Console.WriteLine("create - Creates a new txt file. Args: file name, content (optional)");
		Console.WriteLine("read - Reads an existing file. Args: file name");
		Console.WriteLine("add - Adds content to an existing file. Args: file name, content");
		Console.WriteLine("override - Overrides the content of an existing file. Args: file name, content");
		Console.WriteLine("delete - Deletes an existing file. Args: file name");
		Console.WriteLine("list - Lists the files saved.");
		Console.WriteLine("log - Shows the log of your previous actions and the documents you've unlocked.");
		Console.WriteLine("help - Shows this help menu.");
		Console.WriteLine("exit - Exit the program.");
	}

	public static void Exit() => Environment.Exit(0);
}
