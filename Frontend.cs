using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace vrim
{
	// stateful but immutable after first instantiation
	// maps keypresses to method calls on buffers
	// NOT THREADSAFE
	// TODO read up on static classes in C#, maybe we can just make this a static class
	public sealed class Frontend
	{
		private static Frontend instance = null;
		private string configFilePath = "/Users/Bhargava/Projects/vrim/config.json";
		private Dictionary<string, MethodInfo> keymap;

		private Frontend()
		{
			keymap = new Dictionary<string, MethodInfo> ();
			Dictionary<string, string> pre = JsonConvert.DeserializeObject<Dictionary<string, string>> (File.ReadAllText (configFilePath));
			foreach (String key in pre.Keys) {
				keymap [key] = typeof(Buffer).GetMethod (pre [key]);
			}
		}
		public static Frontend Instance {
			get {
				if (instance == null)
					instance = new Frontend ();
				return instance;
			}
		}

		public void ProcessInput(Buffer b)
		{

		}
	}
}

