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
		private PieceChain _chain;
		private string _filename;

		public Buffer()
		{
			_chain = new PieceChain ();
			_filename = null;
		}

		public Buffer(string filename)
		{
			_filename = filename;
			char[] f;
			try {
				f = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(_filename)).ToCharArray();
			} catch(FileNotFoundException) {
				_chain = new PieceChain ();
				return;
			}
			f = new char[0];
			_chain = new PieceChain (f);
		}
	}

}

