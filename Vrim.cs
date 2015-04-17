using System;

namespace vrim
{
	class Vrim
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Entrypoint to vrim editor");
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (0, "12345");
			chain.Undo ();
			chain.Redo ();
			Console.WriteLine ("it works! maybe...");
		}
	}
}
