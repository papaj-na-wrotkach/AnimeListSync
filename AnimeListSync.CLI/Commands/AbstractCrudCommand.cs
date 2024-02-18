using System.CommandLine;
using System.CommandLine.Binding;
using System.Reflection;
using System.Text.Json;
using AnimeListSync.DB;
using AnimeListSync.DB.Entity.Attribute;
using Dynamitey;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.CLI.Commands;

public abstract class AbstractCrudCommand<TEntity, TId> : Command
	where TEntity : class, IIndetifiable<TId>
{
	public AbstractCrudCommand(
		DbSet<TEntity> dbSet,
		BinderBase<dynamic> createBinder,
		BinderBase<dynamic> updateBinder,
		string name,
		string? description = null) : base(name, description)
	{
		DataSet = dbSet;

		#region initializing binders
		CreateBinder = createBinder;
		UpdateBinder = updateBinder;
		#endregion

		#region setting up command aliases
		CreateCommand.AddAlias("c", "add", "a");
		ListCommand.AddAlias("l");
		RemoveCommand.AddAlias("r", "delete", "d");
		UpdateCommand.AddAlias("u");
		#endregion

		AddCommandArguments();

		#region adding universal arguments
		ListCommand.AddArgument(IdMultiArgument);
		RemoveCommand.AddArgument(IdMultiArgument);
		UpdateCommand.AddArgument(IdSingleArgument);
		#endregion

		#region setting universal handlers
		CreateCommand.SetHandler(createDto =>
		{
			TEntity entity = Dynamic.InvokeConstructor(typeof(TEntity));
			UpdateEntity(ref entity, createDto);
			DataSet.Add(entity);
		}, CreateBinder);
		ListCommand.SetHandler(ids => Console.WriteLine(JsonSerializer.Serialize(ids.Length > 0
			? DataSet.GetByIds(ids)
			: DataSet)), IdMultiArgument);
		RemoveCommand.SetHandler(ids => DataSet.RemoveRange(DataSet.GetByIds(ids)), IdMultiArgument);
		UpdateCommand.SetHandler((id, updateDto) =>
		{
			var old = DataSet.GetById(id) ?? throw new($"Could not find entity with given id: {id}");
			if (old is null) return;
			UpdateEntity(ref old, updateDto);
			DataSet.Update(old);
		}, IdSingleArgument, UpdateBinder);
		#endregion

		this.AddCommandRange(CreateCommand, ListCommand, RemoveCommand, UpdateCommand);
	}

	#region reflections stuff
	private IEnumerable<PropertyInfo> EntityProperties { get; } = typeof(TEntity)
		.GetProperties();
	private void UpdateEntity(ref TEntity entity, dynamic dto)
	{
		foreach (var eprop in EntityProperties) try
		{
			Dynamic.InvokeSet(entity, eprop.Name, Dynamic.InvokeGet(dto, eprop.Name));
		} catch (RuntimeBinderException) {}
	}
	#endregion

	protected DbSet<TEntity> DataSet { get; }

	#region commands
	protected Command CreateCommand { get; } = new("create");
	protected Command ListCommand { get; } = new("list");
	protected Command RemoveCommand { get; } = new("remove");
	protected Command UpdateCommand { get; } = new("update");
	#endregion

	#region binders
	protected BinderBase<dynamic> CreateBinder { get; }
	protected BinderBase<dynamic> UpdateBinder { get; }
	#endregion

	#region arguments
	protected Argument<TId[]> IdMultiArgument { get; } = new("id")
	{
		Arity = ArgumentArity.ZeroOrMore
	};
	protected Argument<TId> IdSingleArgument { get; } = new("id")
	{
		Arity = ArgumentArity.ExactlyOne
	};
	#endregion

	protected abstract void AddCommandArguments();
}
