using MalApi;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeListSync;

public enum ListProvider : byte
{
	[Description("MyAnimeList (https://myanimelist.net)")]
	MyAnimeList,
	[Description("Shinden (https://shinden.pl)")]
	Shinden
}

class Program
{
	private static readonly Config config = Config.FromFile();
	public static MalClient MalClient { get; set; } = new ();
	public static readonly Database Db = new();
	static async Task Main(params string[] args)
	{
		var queryArgument = new Argument<List<string>>(
			name: "query",
			description: "Search query")
		{
			Arity = ArgumentArity.OneOrMore
		};

		var assocCommand = new Command(
			name: "assoc",
			description: "Associate a series between providers")
		{
			queryArgument
		};

		assocCommand.SetHandler(async (List<string> queries) =>
		{
			Logger.Trace("Search keywords: [[");
			foreach (var keyword in queries)
			{
				Logger.Trace($"\t{keyword},");
			}
			Logger.Trace("]]");
			
			var databaseAnime = new Database.DatabaseAnime();
			Db.Add(databaseAnime);

			foreach (ListProvider provider in typeof(ListProvider).GetEnumValues())
			{
				Logger.Info($"Searching using {provider}");
				IEnumerable<Anime> choices;
				switch (provider)
				{
					case ListProvider.Shinden: continue;// throw new NotImplementedException();
					case ListProvider.MyAnimeList or _:
						MalClient.SetAuth(config.MyAnimeList);
						choices = (
							from search in await Task.WhenAll(from query in queries select MalClient.Anime().WithName(query).Find())
							from series in search.Data group series by series.Id into grouped orderby grouped.Count() descending
							from series in grouped select series
						).DistinctBy(series => series.Id);
					break;
					
				};
				
				var choice = AnsiConsole.Prompt(new SelectionPrompt<Anime>()
					{
						Title = $"Choose a {provider} series",
						Converter = anime => $"[link=https://myanimelist.net/anime/{anime.Id}]{anime.Title}[/]"
					}
					.AddChoices(choices));
				Logger.Info($"Choose: {choice}");
				var providerAnime = new Database.ProviderAnime()
				{
					Id = choice.Id,
					Provider = provider
				};
				Db.Add(providerAnime);
				Db.Add<Database.AnimeAssociation>(new()
				{
					Database = databaseAnime,
					Provider = providerAnime
				});
			}
		}, queryArgument);


		var providerArgument = new Argument<ListProvider>(
			name: "provider",
			description: "List provider to use")
		{
			Arity = ArgumentArity.ZeroOrOne,
		};

		var authCommand = new Command(
			name: "auth",
			description: "Authenticate with a list provider")
		{
			providerArgument
		};

		authCommand.SetHandler((ListProvider provider) =>
		{
			Logger.Info($"Authenticating with {provider}");
			switch (provider)
			{
				case ListProvider.Shinden:
					throw new NotImplementedException();
				case ListProvider.MyAnimeList or _:
					config.MyAnimeList = new();
					MalClient.SetAuth(config.MyAnimeList);
					break;
			}
		}, providerArgument);

		var rootCommand = new RootCommand(
			description: "A fire-and-forget CLI app allowing users to sync their anime and manga lists between multiple providers")
		{
			assocCommand,
			authCommand,
		};
		
		await rootCommand.InvokeAsync(args);
        Db.SaveChanges();
		config.Save();
		return;
	}
}
