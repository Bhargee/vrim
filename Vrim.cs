using System;
using System.IO;

namespace vrim
{
	class Vrim
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Entrypoint to vrim editor");
			//Buffer t = new Buffer ("/home/bhargava/dev/class/vrim/test.txt");
			//Frontend f = Frontend.Instance;
			//f.ProcessInput (t);
			char[] fileContents = System.Text.Encoding.UTF8.GetString (File.ReadAllBytes ("/home/bhargava/dev/class/vrim/test.txt")).ToCharArray ();
			PieceChain p = new PieceChain (fileContents);
			p.PrintContentsTesting ();
			Console.WriteLine ("");
			p.Insert (fileContents.Length-1, "FUCKTHEPOLICE");
			p.Insert (18, "DUCKDUCKGO");
			p.PrintContentsTesting ();
			Console.WriteLine ("it works! maybe...");
		}
	}
}
