using System.CommandLine;
using AnimeListSync.CLI.CommandModule;
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
		var rootCommand = new RootCommand
		{
			new InternalSeriesCommand(Db).Command
		};

		await rootCommand.InvokeAsync(args);
		Db.SaveChanges();
		return;
	}
}
