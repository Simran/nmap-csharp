using System;
using System.Collections.Generic;
namespace nmapcsharp
{
	public class Functions
	{
		static Dictionary<int, ConsoleColor> colors = new Dictionary<int, ConsoleColor>();
		public static void InitColors()
		{
			colors.Add(1, ConsoleColor.Blue);
			colors.Add(2, ConsoleColor.Green);
			colors.Add(3, ConsoleColor.Red);
			colors.Add(4, ConsoleColor.Yellow);
			colors.Add(5, ConsoleColor.Cyan);
		}
			
		public static void log(string data, int type)
		{
			Console.Write("[{0}]", DateTime.Now);
			Console.ForegroundColor = colors[type];
			Console.WriteLine(data);
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}

