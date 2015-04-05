using System;

namespace vrim
{
	class Vrim
	{
		private enum Modes {Insert, Command, Visual};
		public static void Main (string[] args)
		{
			Console.WriteLine ("Entrypoint to vrim editor");
			Buffer t = new Buffer ("/home/bhargava/dev/class/vrim/test.txt");
			Frontend f = Frontend.Instance;
			f.ProcessInput (t);
			Console.WriteLine ("it works! maybe...");
		}
	}
}
