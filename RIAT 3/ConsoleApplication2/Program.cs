using System;

namespace ConsoleApplication2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ISerialize serialize = new JsonSerialize();
			string domain = "127.0.0.1";
			int port = int.Parse(Console.ReadLine());

			HttpServer server = new HttpServer(serialize, domain, port);
			server.Listen();
		}
	}
}
