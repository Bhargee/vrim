using System;

namespace vrim
{
	class Vrim
	{
		public static void Main (string[] args)
		{
			PieceChain chain = new PieceChain (new char[] { 'a', 'b', 'c', 'd', 'e' });
			chain.Insert (0, "123");
			Console.WriteLine(chain.GetContentsTesting ());
			chain.Undo ();
			Console.WriteLine (chain.GetContentsTesting());
			chain.Delete (2, 2);
			Console.WriteLine (chain.GetContentsTesting ());

		}
	}
}
