using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Slusser.Collections.Generic;

namespace vrim
{

	/* 
	 * Class Buffer manages the "backend" of vrim, exposing an API for basic text manipulation
	 * (moving the point, entering chars, deleting chars, etc)
	 * there is a bijection between open files and Buffer objects
	 */
	public class Buffer
	{
		private GapBuffer<char> buffer;
		private string filename =  null;
		private int point = 0;
		private bool dirty = false; // set to true when the buffer is edited, and adjust actions accordingly
		private string displayCache = null;

		private int linewidth = 80;
		public int Linewidth {
			set { this.linewidth = value;}
		}

		public enum Direction {Up, Left, Right, Down};

		public string Filename {
			get { return this.filename;}
			set { this.filename = value;}
		}
			
		public Buffer ()
		{
			buffer = new GapBuffer<char>();
		}

		public Buffer (String filename) 
		{
			buffer = new GapBuffer<char> ();
			buffer.InsertRange (point, File.ReadAllText (filename));  
			this.filename = filename;
		}

		// Public API
		public string AsString ()
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
			dirty = true;
			return point;
		}

		public int Remove(int num) 
		{
			buffer.RemoveRange (point, num);
			dirty = true;
			return point;
		}
		public List<int> RegexSearch(string pattern)
		{
			List<int> res = new List<int>();
			Regex rgx = new Regex(pattern);
			
			foreach (Match match in rgx.Matches(this.AsString()))
				res.Add (match.Index);
			
			return res;
		}
		
		public int BufferEnd()
		{
			point = buffer.Count - 1;
			return point;
		}
		
		public void BufferStart()
		{
			point = 0;
		}
		
		public int LineEnd()
		{
			point = (linewidth * (int)(Math.Ceiling ((double)point / linewidth))) - 1;
			return point;
		}
		
		public int LineStart()
		{
			point = linewidth * (int)(Math.Floor ((double) point / linewidth));
			return point;
		}
		
		// TODO need to signal if write fails because of dirty bit or nil filename
		public void Write() 
		{
			// sanity check, display needs to be updated before we write to disk
			if (dirty || filename == null)
				return;
			File.WriteAllText (this.filename, this.AsString());
		}

		public int MovePointUp(int amount)
		{
			return MovePoint (Direction.Up, amount);
		}

		public int MovePointRight(int amount)
		{
			return MovePoint (Direction.Right, amount);
		}

		public int MovePointDown(int amount)
		{
			return MovePoint (Direction.Down, amount);
		}

		public int MovePointLeft(int amount)
		{
			return MovePoint (Direction.Left, amount);
		}
		//Private helpsers

		private int MovePoint(Direction d, int amount)
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

		//TODO move to display component
		/*public string Display(bool windows) 
		{
			if (!dirty && displayCache != null) {
				return displayCache;
			}
			string text = this.AsString ();
			List<string> lines = new List<string>();
			if (windows)
				lines.AddRange (Regex.Split (text, "\r\n"));
			else
				lines.AddRange(text.Split('\n'));
			for (int i = 0; i < lines.Count; i++) {
				if(lines[i].Length >= linewidth) {
					string oldline = lines[i];
					lines[i] = oldline.Substring(0, linewidth - 1);
					lines.Insert(i+1, oldline.Substring(linewidth, oldline.Length-1));
				}
			}
			displayCache = String.Join ("\n", lines);
			dirty = false;
			return displayCache;
		}*/

	}
}

