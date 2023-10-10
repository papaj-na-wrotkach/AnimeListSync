using System;
using System.IO;

static class Constants {
	public static readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnimeListSync");
	public static readonly string configPath = Path.Combine(configDir, "config.yml");
}