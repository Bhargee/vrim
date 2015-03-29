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
			while (true) {
				string line = Console.ReadLine();
				if (line.Contains("q"))
					break;
				else if (line.Equals("l"))
					t.MovePoint(Buffer.Direction.Right, 1);
				else if (line.Equals("k"))
					t.MovePoint(Buffer.Direction.Up, 1);
				else if (line.Equals("j"))
					t.MovePoint(Buffer.Direction.Down, 1);
				else if (line.Equals("h"))
					t.MovePoint(Buffer.Direction.Left, 1);
				/*else if (line.Equals("w"))
					Console.WriteLine(t.GetWord());
				else
					Console.WriteLine(t.GetChar());*/
			}
			Console.Write (t.Display (false));
			/*Application.Init (false);
			Frame f = new Frame ("Test Frame");
			f.Add (new Label (0, 0, "Hello World"));
			Application.Run (f);*/

		}
	}
}
