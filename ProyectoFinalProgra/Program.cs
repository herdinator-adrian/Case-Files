using System;
using System.Data;

public class Program
{

	public static void Main()
	{
		DocumentManager.InitializeGame();

		Console.WriteLine("=== Welcome to Text File Manager ===");
		Commands.Help();
		Console.WriteLine("You can leave anytime using the command \"exit\" or Ctrl+C.\n");

		while (true)
		{
			Console.Write("> ");
			string? input = Console.ReadLine();
			if (input == null) continue;
			input = input.Trim();

			string[] splittedInput = input.Split(' ', 2);
			string command = splittedInput[0].ToLower();
			string[] args = { };
			if (splittedInput.Length > 1) args = splittedInput[1].Split(' ');

			Commands.HandleCommand(command, args);
			Console.WriteLine();
		}
	}
}