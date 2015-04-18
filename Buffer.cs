using System;
using System.IO;

namespace vrim
{

	/* 
	 * Class Buffer manages the "backend" of vrim, exposing an API for basic text manipulation
	 * (moving the point, entering chars, deleting chars, etc)
	 * there is a bijection between open files and Buffer objects
	 */
	public class Buffer
	{
		private PieceChain chain;
		private string filename;
		private int lineLen =  40;
		private int cursor = 0;

		private enum Movement {Down, Up, Left, Right};

		public Buffer()
		{
			chain = new PieceChain ();
			filename = null;
		}

		public Buffer(string filename)
		{
			this.filename = filename;
			char[] f;
			try {
				f = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(filename)).ToCharArray();
			} catch(FileNotFoundException) {
				chain = new PieceChain ();
				return;
			}
			chain = new PieceChain (f);
		}

		public void Insert(string toInsert)
		{
			if (chain.Insert (cursor, toInsert))
				cursor += toInsert.Length;
		}

		public void Delete(int length)
		{
			chain.Delete (cursor, length);
		}
	}

}

