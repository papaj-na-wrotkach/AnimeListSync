using System.CommandLine;
using AnimeListSync.DB;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.CLI.Commands;

public abstract class AbstractCrudCommand<TEntity, TId> : Command
	where TEntity : class, IIndetifiable<TId>
	where TId : struct
{
	public AbstractCrudCommand(string name, DbSet<TEntity> dbSet, string? description = null) : base(name, description)
	{
		DbSet = dbSet;
		SetupCommands();
		this.AddCommandRange(CreateCommand, QueryCommand, RemoveCommand, UpdateCommand);
	}

	protected virtual DbSet<TEntity> DbSet { get; }
	protected Command CreateCommand { get; } = new Command("create");
	protected Command QueryCommand { get; } = new Command("query");
	protected Command RemoveCommand { get; } = new Command("remove");
	protected Command UpdateCommand { get; } = new Command("update");
	protected virtual Argument<TId> IdArgument { get; set; } = new("id")
	{
		Arity = ArgumentArity.ExactlyOne, //TODO: use positional args for ability to change multiple entities at once
	};
	protected virtual Option<TId?> IdZeroOrOneOption { get; set; } = new(["--id", "-i"])
	{
		Arity = ArgumentArity.ZeroOrOne
	};
	internal void SetupCommands()
	{
		CreateCommand.AddAlias("c", "add", "a");
		QueryCommand.AddAlias("q", "search", "s");
		RemoveCommand.AddAlias("r", "delete", "d");
		UpdateCommand.AddAlias("u");

		
		SetupArguments();
		RemoveCommand.AddArgument(IdArgument);
		UpdateCommand.AddArgument(IdArgument);
		UpdateCommand.AddOption(IdZeroOrOneOption);
		SetupHandlers();
	}
	protected abstract void SetupArguments();
	protected abstract void SetupHandlers();
}
