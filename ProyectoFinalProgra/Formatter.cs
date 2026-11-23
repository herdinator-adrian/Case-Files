using System;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

public static class Formatter
{
	const int textWriteAnimationPause = 10;

	public enum BlockCommand
	{
		HEADER,
		SUBTITLE,
		DIALOGUE,
		PAUSE,
		LINE,
		COMMAND,
		UNLOCK,
		TEXT,
	}

	public static void ProcessText(string text)
	{
		string[] blocks = text.SplitDoubleLines();

		foreach (string block in blocks)
		{
			HandleBlockCommands(block);
		}
		Console.WriteLine();
	}

	/// <summary>
	/// Handles text chunks/blocks in a string separated by a double line (\n\n), checking for commands written in [].
	/// Example:
	/// 
	/// [dialogue]
	/// Hello.
	/// How are you?
	/// 
	/// This text is not part of the dialogue.
	/// </summary>
	/// <param name="block"></param>
	/// <exception cref="Exception"></exception>
	private static void HandleBlockCommands(string block)
	{
		(BlockCommand command, string[]? args, string? textToApply) = GetBlockCommand(block);

		switch (command)
		{
			case BlockCommand.PAUSE:
				float time = Convert.ToSingle(args![0]);
				Wait(time);
				//Thread.Sleep((int)(time * 1000));
				break;
			case BlockCommand.DIALOGUE:
				if (textToApply != null)
					WriteDialogue(textToApply);
				break;
			case BlockCommand.HEADER:
				if (textToApply != null)
					WriteHeader(textToApply);
				break;
			case BlockCommand.LINE:
				DrawLine();
				break;
			case BlockCommand.UNLOCK:
				if (args == null || args.Length == 0) throw new ArgumentOutOfRangeException("You gave no args while calling unlock.");
				Commands.Unlock(args);
				break;
			case BlockCommand.SUBTITLE:
				if (textToApply != null)
					WriteSubtitle(textToApply);
				break;
			/*case BlockCommand.COMMAND:
				if (args == null || args.Length == 0) throw new ArgumentOutOfRangeException("You gave no args while calling a command.");
				Commands.HandleCommand(args[0], args[1..]);
				break;*/
			default:
				if (textToApply != null)
					WriteLine(textToApply);
				break;
		}
	}

	public static (BlockCommand type, string[]? args, string? textToApply) GetBlockCommand(string block)
	{
		if (block.Contains(Environment.NewLine + Environment.NewLine))
			throw new Exception("There is a double line in this text. The format handler can only handle blocks with max 1 line.");

		string[] lines = block.Split("\n", 2);

		string? formatCommand = FindInBetween(lines[0], '[', ']');
		if (formatCommand == null)
		{
			return (BlockCommand.TEXT, null, block);
		}

		string command = formatCommand;
		string[]? args = null;
		if (formatCommand.Contains("="))
		{
			command = formatCommand.Split("=")[0].Trim();
			args = formatCommand.Split("=")[1].Split(",").Select(x => x.Trim()).ToArray();
		}

		BlockCommand formatType = Enum.Parse<BlockCommand>(command.Trim(), true);

		string? textToApply = null;

		if (lines.Length > 1)
			textToApply = lines[1];

		return (formatType, args, textToApply);
	}

	const int charsToWritePerWait = 5;
	/// <summary>
	/// Writes a new line animated.
	/// </summary>
	/// <param name="text"></param>
	public static void WriteLine(string text, bool animated = true, int animationSpeed = charsToWritePerWait)
	{
		Console.WriteLine();

		if (!animated)
		{
			Console.WriteLine(text);
			return;
		}
		string[] words = text.Split(' ');

		foreach (string word in words)
		{
			if (Console.CursorLeft + word.Length >= Console.WindowWidth)
			{
				Console.WriteLine();
			}
				
			string newWord = word + " ";
			int i = 0;
			while (i < newWord.Length)
			{
				Thread.Sleep(textWriteAnimationPause);
				for (int j = 0; j < animationSpeed; j++)
				{
					if (i >= newWord.Length) break;

					Console.Write(newWord[i]);
					i++;
				}
			}
			
		}

		Console.WriteLine();
	}


	/// <summary>
	/// Pausa el hilo durante el tiempo especificado en milisegundos.
	/// </summary>
	/// <param name="time">time: Tiempo en milisegundos durante el cual el hilo debe esperar.</param>
	public static void Wait(float time)
	{
		Console.WriteLine();
		int reps = 3;
		int j = 0;
		for (int i = 0; i < (int) time * 1000; i += 300)
		{
			Thread.Sleep(300);

			if (j >= reps)
			{
				Console.WriteLine();
				DeleteLine();
				j = 0;
				continue;
			}

			Console.Write(".");
			j++;
		}
		Console.WriteLine();
		DeleteLine();
	}

	/// <summary>
	/// Deletes the last written line in the console.
	/// </summary>
	public static void DeleteLine()
	{
		Console.SetCursorPosition(0, Console.CursorTop - 1);
		Console.Write(new string(' ', Console.WindowWidth));
		Console.SetCursorPosition(0, Console.CursorTop);
	}

	/// <summary>
	/// Writes lines one by one like a dialogue box.
	/// </summary>
	/// <param name="text"></param>
	public static void WriteDialogue(string text)
	{
		(string? speaker, string content)[] dialogues = text.SplitDialogue();

		bool skip = false;
		for (int i = 0; i < dialogues.Length; i++)
		{
			if (!skip)
				Thread.Sleep(100);

			if (dialogues[i].speaker != null)
				WriteLine($"[{dialogues[i].speaker!.ToUpper()}]", !skip, 1);
			WriteLine(dialogues[i].content + "\n", !skip, 1);

			//Console.WriteLine();

			if (skip || i == dialogues.Length - 1) continue;

			//Console.WriteLine("\t\t\t[ENTER] Continue    |    [SPACE] Skip");
			ConsoleKey key = Console.ReadKey(true).Key;
			//DeleteLine();

			if (key == ConsoleKey.Spacebar) skip = true;
		}
	}

	/// <summary>
	/// Formats text to a header.
	/// </summary>
	/// <param name="text"></param>
	public static void WriteHeader(string text) => WriteLine($"""
			
			==================================================
			{text}
			==================================================
			""");

	/// <summary>
	/// Formats text to a subtitle
	/// </summary>
	/// <param name="text"></param>
	public static void WriteSubtitle(string text)
	{
		Console.WriteLine();
		string subTitle = $"--- {text} "; 
		string spaces = new string('-', Math.Max(3, 45 - text.Length));
		WriteLine(subTitle + spaces);
		Console.WriteLine();
	}

	/// <summary>
	/// Asks a question and returns true if yes, false if no.
	/// </summary>
	/// <param name="text">The question to ask. This function will append '(Y/n)' at the end.</param>
	/// <param name="defaultToNo">If true, if the player doesn't answer with a 'y' the answer will be no. Viseversa if false.</param>
	/// <returns></returns>
	public static bool WriteYesNoQuestion(string text, bool defaultToNo = true)
	{
		Console.WriteLine();
		WriteLine(text + " (Y/n)");
		Console.WriteLine();

		Console.Write("> ");
		string answer = Console.ReadLine().Trim().ToLower();
		if (defaultToNo && answer == "y")
			return true;
		else if (!defaultToNo && answer != "n")
			return true;
		return false;
	}

	/// <summary>
	/// Draws a line of - in the console
	/// </summary>
	/// <param name="animated"></param>
	public static void DrawLine(bool animated = true)
	{
		Console.WriteLine();
		for (int i = 0; i < 50; i++)
		{
			if (animated)
				Thread.Sleep(textWriteAnimationPause);
			Console.Write("-");
		}
	}

	/// <summary>
	/// Returns a text in format of dialogue, splitting every line.
	/// It returns a queue with two strings: the name of the speaker and the content.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	private static (string?, string)[] SplitDialogue(this string text)
	{
		string[] dialogues = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
		(string?, string)[] speakerContentPair = new (string?, string)[dialogues.Length];

		for (int i = 0; i < speakerContentPair.Length; i++)
		{
			string line = dialogues[i];
			if (line.IsWhiteSpace()) continue;

			string? speaker = null;
			string content = line.Trim();
			if (line.Contains(":"))
			{
				speaker = line.Split(":", 2)[0].Trim();
				content = line.Split(":", 2)[1].Trim();
			}

			speakerContentPair[i] = (speaker, content);
		}

		return speakerContentPair;
	}

	#region HELPER
	private static string? FindInBetween(string text, char start, char end, bool includeStartAndEndChars = false)
	{
		text = text.Trim();
		int startIdx = text.IndexOf(start, 0);
		int endIdx = text.IndexOf(end, text.Length - startIdx - 1);

		if (startIdx == -1) return null;
		if (startIdx > endIdx) 
			throw new Exception("The start index is larger than the end index.");
		
		string sub = text.Substring(
			includeStartAndEndChars ? startIdx : startIdx + 1,
			includeStartAndEndChars ? endIdx - startIdx : endIdx - startIdx - 1
			);

		return sub;
	}

	public static string[] SplitDoubleLines(this string text) => Regex.Split(text, @"\r?\n\s*\r?\n")
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim())
			.ToArray();

	public static string[] SplitLines(this string text) => Regex.Split(text, @"\r?\n")
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim())
			.ToArray();
	#endregion
}
