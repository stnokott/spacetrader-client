using System;
using System.IO;
using Godot;

namespace Server;

public static class DotEnv
{
	private static readonly string FILEPATH = ".env";

	private static bool loaded = false;
	public static void Load()
	{
		if (!File.Exists(FILEPATH))
		{
			return;
		}

		foreach (var line in File.ReadAllLines(FILEPATH))
		{
			var parts = line.Split("=", System.StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 2)
			{
				continue;
			}
			OS.SetEnvironment(parts[0], parts[1]);
		}
		loaded = true;
	}

	public static string Get(string key)
	{
		if (!loaded)
		{
			Load();
		}
		var v = OS.GetEnvironment(key);
		if (v == "")
		{
			throw new ArgumentException("environment variable " + key + " not found in " + FILEPATH);
		}
		return v;
	}
}
