using System;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			//Nedeterm Nedet = new Nedeterm();
			//Nedet.Input();
			//Nedet.Output();
			try
			{
				Min A = new Min();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			Console.WriteLine("Конец");
			Console.ReadKey();
		}
	}
}
