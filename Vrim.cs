using System;

namespace vrim
{
	class Vrim
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Entrypoint to vrim editor");
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (5, "12345");
			//chain.Undo ();
			Console.WriteLine (chain.GetContentsTesting ());
			chain.Insert (3, "fghij");
			Console.WriteLine (chain.GetContentsTesting ());
			Console.WriteLine ("it works! maybe...");
		}
	}
}
