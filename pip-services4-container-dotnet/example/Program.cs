using System;

namespace PipServices4.Container
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var process = new DummyProcess();
				process.RunAsync(args).Wait();

				Console.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
