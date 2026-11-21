using System;

public static class Helper
{
	public static void PrintArray<T>(this T[] arr) where T : notnull
	{
		Console.Write("[");
		foreach (T item in arr)
		{
			Console.Write(item.ToString() + ", ");
		}
		Console.Write("]");
	}
}
