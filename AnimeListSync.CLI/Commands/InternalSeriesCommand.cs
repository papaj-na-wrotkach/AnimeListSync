using System.CommandLine;
using System.Text.Json;
using System.Text.RegularExpressions;
using AnimeListSync.DB;
using AnimeListSync.DB.Entity;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.CLI.Commands;

public class InternalSeriesCommand(
	string name,
	DbSet<InternalSeriesEntity> dbSet,
	string? description = null)
	: AbstractCrudCommand<InternalSeriesEntity, long>(name, dbSet, description)
{
	private static string[] NameAliases { get; } = ["--name", "-n"];
	private Option<string> NameRequiredSingleOption { get; } = new(NameAliases)
	{
		Arity = ArgumentArity.ExactlyOne,
		IsRequired = true
	};
	private Option<string?> NameSingleOption { get; } = new(NameAliases)
	{
		Arity = ArgumentArity.ExactlyOne
	};
	private Option<string> DescriptionSingleOption { get; } = new(["--description", "-d"])
	{
		Arity = ArgumentArity.ExactlyOne
	};
	private Argument<Regex?> RegexZeroOrOneArgument { get; } = new("regex")
	{
		Arity = ArgumentArity.ZeroOrOne
	};
	protected override void SetupArguments()
	{
		DescriptionSingleOption.SetDefaultValue(string.Empty);

		CreateCommand.Add(NameRequiredSingleOption, DescriptionSingleOption);
		QueryCommand.Add(RegexZeroOrOneArgument);
	}

	protected override void SetupHandlers()
	{
		CreateCommand.SetHandler((name, description) => DbSet.Add(new InternalSeriesEntity { Name = name, Description = description }),
			NameRequiredSingleOption,
			DescriptionSingleOption);

		QueryCommand.SetHandler(regex => Console.WriteLine(JsonSerializer.Serialize(DbSet
			.AsEnumerable()
			.OrderByDescending(series => regex?.Matches(series.Name).Count ?? 0))),
			RegexZeroOrOneArgument);

		RemoveCommand.SetHandler(id => {
			var entity = DbSet.GetById(id);
			if (entity is null) return;
			DbSet.Remove(entity);
		}, IdArgument);

		UpdateCommand.SetHandler((id, newId, name, description) =>
		{
			var entity = DbSet.GetById(id);
			if (entity is null) return;
			if (newId is long notnullNewId) entity.Id = notnullNewId;
			if (name is not null) entity.Name = name;
			if (description is not null) entity.Description = description;
		}, IdArgument, IdZeroOrOneOption, NameSingleOption, DescriptionSingleOption);
	}
}