using System;

namespace vrim
{
	class Vrim
	{
		private enum Modes {Insert, Command, Visual};
		public static void Main (string[] args)
		{
			Console.WriteLine ("Entrypoint to vrim editor");
			Buffer t = new Buffer ("/Users/Bhargava/Projects/Vrim/test.txt");
			Frontend f = Frontend.Instance;
			Console.WriteLine ("it works! maybe...");
		}
	}
}
