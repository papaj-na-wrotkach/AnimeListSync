using System.CommandLine;
using AnimeListSync.CLI.Commands;
using AnimeListSync.Config;
using AnimeListSync.DB;

namespace AnimeListSync.CLI;

class Program
{
	private static readonly ConfigInstance config = ConfigInstance.FromFile();
	// public static MalClient MalClient { get; set; } = new();
	public static readonly Database Db = new() { DbPath = Constants.dbPath };
	static async Task Main(params string[] args)
	{
		Db.Database.EnsureCreated();
		var seriesCommand = new InternalSeriesCommand("series", Db.InternalSeriesSet);
		var rootCommand = new RootCommand
		{
			seriesCommand
		};

		await rootCommand.InvokeAsync(args);
		Db.SaveChanges();
		return;
	}
}
