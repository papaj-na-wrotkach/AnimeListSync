using System.CommandLine;
using System.Text.Json;
using System.Text.RegularExpressions;
using AnimeListSync.DB.Entity;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.CLI.Commands;

public class InternalSeriesCommand : AbstractCrudCommand<InternalSeriesEntity, long>
{
	private Command QueryCommand { get; } = new("query");

	private static Argument<string> NameArgument { get; } = new("name")
	{
		Arity = ArgumentArity.ExactlyOne
	};
	private static Argument<string?> DescriptionArgument { get; } = new("description")
	{
		Arity = ArgumentArity.ZeroOrOne
	};

	private static Option<long?> IdOption { get; } = new(["--id", "-i"])
	{
		Arity = ArgumentArity.ExactlyOne
	};
	private static Option<string?> NameOption { get; } = new(["--name", "-n"])
	{
		Arity = ArgumentArity.ExactlyOne
	};
	private static Option<string?> DescriptionOption { get; } = new(["--description", "-d"])
	{
		Arity = ArgumentArity.ZeroOrOne
	};

	private Argument<Regex> RegexArgument { get; } = new(
		name: "regex",
		parse: result => new Regex(result.Tokens[0].Value))
	{
		Arity = ArgumentArity.ExactlyOne
	};

	public InternalSeriesCommand(
		DbSet<InternalSeriesEntity> dbSet,
		string name,
		string? description = null) : base(
			dbSet,
			new InternalSeriesCreateBinder
			{
				NameArgument = NameArgument,
				DescriptionArgument = DescriptionArgument
			},
			new InternalSeriesUpdateBinder
			{
				IdOption = IdOption,
				NameOption = NameOption,
				DescriptionOption = DescriptionOption
			},
			name,
			description)
	{
		// additional commands
		QueryCommand.AddAlias("q", "search", "s");
		QueryCommand.AddArgument(RegexArgument);
		QueryCommand.SetHandler(regex => Console.WriteLine(JsonSerializer.Serialize(DataSet
			.AsEnumerable()
			.Aggregate(new List<dynamic>(), (acc, series) =>
			{
				var matches = regex.Matches(series.Name);
				if (matches.Count > 0) acc.Add(new { Series = series, Matches = matches.Count });
				return acc;
			})
			.OrderByDescending(obj => obj.Matches)
			.Select(obj => obj.Series))),
			RegexArgument);

		AddCommand(QueryCommand);
	}

	protected override void AddCommandArguments()
	{
		CreateCommand.AddArgumentRange(NameArgument, DescriptionArgument);
		UpdateCommand.AddOptionRange(IdOption, NameOption, DescriptionOption);
	}
}