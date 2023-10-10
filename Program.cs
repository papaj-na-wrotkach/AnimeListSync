using MalApi;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeListSync;

public enum ListProvider
{
	MyAnimeList,
	Shinden
}

class Program
{
	private static readonly Config config = Config.FromFile();
	public static MalClient MalClient { get; set; } = new ();

	static async Task Main(params string[] args)
	{
		var providerOption = new Option<ListProvider>(
			name: "--provider",
			description: "List provider to use",
			getDefaultValue: () => ListProvider.MyAnimeList)
		{
			Arity = ArgumentArity.ExactlyOne,
			AllowMultipleArgumentsPerToken = false,
			IsRequired = false
		};
		providerOption.AddAlias("-p");

		var providerArgument = new Argument<ListProvider>(
			name: "provider",
			description: "List provider to use")
		{
			Arity = ArgumentArity.ZeroOrOne,
		};

		var queryArgument = new Argument<List<string>>(
			name: "query",
			description: "Search query")
		{
			Arity = ArgumentArity.OneOrMore
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

		var searchCommand = new Command(
			name: "search",
			description: "Search for a anime series")
		{
			queryArgument,
			providerOption
		};

		searchCommand.SetHandler(async (ListProvider provider, List<string> queries) =>
		{
			IEnumerable<Anime> choices;
			Logger.Info($"Searching using {provider}");
			Logger.Trace("Search keywords: [[");
			foreach (var keyword in queries)
			{
				Logger.Trace($"\t{keyword},");
			}
			Logger.Trace("]]");

			switch (provider)
			{
				case ListProvider.Shinden: throw new NotImplementedException();
				case ListProvider.MyAnimeList or _:
					MalClient.SetAuth(config.MyAnimeList);
					choices = (
						from search in await Task.WhenAll(from query in queries select MalClient.Anime().WithName(query).Find())
						from series in search.Data group series by series.Id into grouped orderby grouped.Count() descending
						from series in grouped select series
					).DistinctBy(series => series.Id);
				break;
				
			};

			var choice = AnsiConsole.Prompt(new SelectionPrompt<Anime>().AddChoices(choices));
			Logger.Info($"Choose: {choice}");
		}, providerOption, queryArgument);

		var rootCommand = new RootCommand(
			description: "A fire-and-forget CLI app allowing users to sync their anime and manga lists between multiple providers")
		{
			authCommand,
			searchCommand
		};
		
		await rootCommand.InvokeAsync(args);
		config.Save();
		return;
	}
}
