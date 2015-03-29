using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Slusser.Collections.Generic;

namespace vrim
{

	/* 
	 * Class Text manages the "backend" of vrim, exposing an API for basic text manipulation
	 * (moving the point, entering chars, deleting chars, etc)
	 * there is a bijection between open files and Text objects
	 */
	public class Text
	{
		private GapBuffer<char> buffer;
		private string filename =  null;
		private int point = 0;

		private static int linewidth = 80;

		public enum Direction {Up, Left, Right, Down};

		public string Filename {
			get { return this.filename;}
			set { this.filename = value;}
		}

		public int Point {
			get { return this.point;}
			set { this.point = value;}
		}

		public Text ()
		{
			buffer = new GapBuffer<char>();
		}

		public Text (String filename) 
		{
			buffer = new GapBuffer<char> ();
			buffer.InsertRange (point, File.ReadAllText (filename));  
			this.filename = filename;
		}

		public override string ToString ()
		{
			char [] res_array = new char[buffer.Count];
			buffer.CopyTo (res_array, 0);
			string res = new string (res_array);
			return res;
		}

		public int Insert(string data)
		{
			if (point == buffer.Count) 
				buffer.AddRange (data);
			else
				buffer.InsertRange (point, data);

			point += data.Length;
			return point;
		}

		public int MovePoint(Direction d, int amount)
		{
			if (d == Direction.Up) {
				if ((point - (amount * linewidth)) >= 0) {
					point -= (amount * linewidth);
				}
			} else if (d == Direction.Right) {
				if ((point + amount) < buffer.Count) {
					point += amount;
				}
			} else if (d == Direction.Down) {
				if ((point + (amount * linewidth)) < buffer.Count) {
					point += (amount * linewidth);
				}
			} else if (d == Direction.Left) {
				if ((point - amount) >= 0) {
					point -= amount;
				}
			}
			return point;
		}

		public char GetChar() 
		{
			return buffer [this.point];
		}

		public string Display(bool windows) 
		{
			int added = 0;
			char newline = '\n';
			if (windows)
				newline = '\r';
			int num_newlines = buffer.Count / linewidth;
			char[] res = new char[buffer.Count + num_newlines];
			for (int i = 0; i < buffer.Count-1; i++) {
				if ((i % linewidth) == 0) {
					res[i+added] = newline;
					added++;
					res[i+added] = buffer[i];
				}
				else
					res[i+added] = buffer[i];
			}
			return new string(res);
		}

		public List<int> RegexSearch(string pattern)
		{
			List<int> res = new List<int>();
			Regex rgx = new Regex(pattern);

			foreach (Match match in rgx.Matches(this.ToString()))
				res.Add (match.Index);

			return res;

		}

		public void Write() 
		{
			File.WriteAllText (this.filename, this.Display (false));
		}
	}
}

