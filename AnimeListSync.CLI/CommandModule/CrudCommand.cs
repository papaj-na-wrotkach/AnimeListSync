using System.CommandLine;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using AnimeListSync.CLI;
using AnimeListSync.DB;
using AnimeListSync.DB.Entity;
using AnimeListSync.DB.Entity.Attribute;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.CLI.CommandModule;

public class CrudCommand<TEntity, TId> where
	TEntity : class, IIndetifiable<TId>
{
	public Command Command { get; }
	public Command Query { get; }
	public Command Add { get; }
	public Command Update { get; }
	public Command Remove { get; }

	private readonly DbSet<TEntity> _dbSet;
	private readonly IEnumerable<PropertyInfo> _editableProperties = typeof(TEntity)
		.GetProperties(BindingFlags.Public & BindingFlags.GetProperty & BindingFlags.SetProperty & BindingFlags.Instance)
		.Where(prop =>
			((CrudEditableAttribute?)prop.GetCustomAttribute(typeof(CrudEditableAttribute)))
				?.AllowEdit ?? false);

	public delegate void VarargHandler(params object[] objects);
	public CrudCommand(DbSet<TEntity> dbSet)
	{
		_dbSet = dbSet;

		// query subcommand
		{
			var options = _editableProperties
				.Aggregate(new Dictionary<string, Option>(), (acc, prop) =>
				{
					var name = prop.Name.ToLower(); // TODO: filter bad characters and use attributes to specify name excliptly
					string[] aliases = [$"--{name}", $"-{name[0]}"];
					acc.Add(prop.Name, Type.GetTypeCode(prop.PropertyType) switch
						{
							TypeCode.Boolean => new Option<bool?>(aliases),
							TypeCode.Byte => new Option<byte?>(aliases),
							TypeCode.Char => new Option<char?>(aliases),
							TypeCode.Decimal => new Option<decimal?>(aliases),
							TypeCode.Double => new Option<double?>(aliases),
							TypeCode.Int16 => new Option<short?>(aliases),
							TypeCode.Int32 => new Option<int?>(aliases),
							TypeCode.Int64 => new Option<long?>(aliases),
							TypeCode.SByte => new Option<sbyte?>(aliases),
							TypeCode.Single => new Option<float?>(aliases),
							TypeCode.String => new Option<string?>(aliases),
							TypeCode.UInt16 => new Option<ushort?>(aliases),
							TypeCode.UInt32 => new Option<uint?>(aliases),
							TypeCode.UInt64 => new Option<ulong?>(aliases),
							_ => throw new NotImplementedException(),
						});

					return acc;
				});

			Query = new("query")
			{
				options.Values
			};
			Query.SetHandler(async (params object?[] arguments) =>
			{
				return Task.CompletedTask;
			}, options.Values);(params object[]) =>
			{
				var nameRegex = name is null ? null : new Regex(name);
				var descriptionRegex = description is null ? null : new Regex(description);

				Console.WriteLine(JsonSerializer.Serialize(_dbSet
					.AsEnumerable()
					.Where(ids.Length > 0
						? entity => ids.Contains(entity.Id) //TODO: optimize for big id sets by removing id if already matched (ids are unique)
						: entity => (nameRegex?.IsMatch(entity.Name) ?? true) && (descriptionRegex?.IsMatch(entity.Description) ?? true))));
			}, options["id"]);
			foreach (var alias in (string[])[ "q", "search", "s" ])
				Query.AddAlias(alias);
		}
		
		// add subcommand
		{
			var nameOption = new Option<string>(["--name", "-n"])
			{
				Arity = ArgumentArity.ExactlyOne,
				IsRequired = true
			};
			var descriptionOption = new Option<string?>(["--description", "-d"])
			{
				Arity = ArgumentArity.ExactlyOne,
				IsRequired = false
			};
			Add = new(
				name: "add")
			{
				nameOption,
				descriptionOption
			};
			Add.SetHandler((name, description) =>
				_db.Add(new InternalSeriesEntity
				{
					Name = name,
					Description = description ?? string.Empty
				}), nameOption, descriptionOption);
			Add.AddAlias("a");
		}
		
		// update subcommand
		{
			var idArgument = new Argument<TId>("id")
			{
				Arity = ArgumentArity.ExactlyOne
			};
			var idOption = new Option<long?>(["--id", "-i"])
			{
				Arity = ArgumentArity.ExactlyOne,
			};
			var nameOption = new Option<string?>(["-n", "--name"])
			{
				Arity = ArgumentArity.ExactlyOne
			};
			var descriptionOption = new Option<string?>(["-d", "--description"])
			{
				Arity = ArgumentArity.ExactlyOne
			};
			Update = new(
				name: "update")
			{
				idArgument,
				idOption,
				nameOption,
				descriptionOption
			};
			Update.SetHandler((id, newId, name, description) => 
			{
				var entity = _dbSet.GetById(id);
				if (entity is null) return;
				if (newId is not null)
					entity.Id = (long)newId;
				if (name is not null)
					entity.Name = name;
				if (description is not null)
					entity.Description = description;
				_dbSet.Update(entity);
			}, idArgument, idOption, nameOption, descriptionOption);
			Update.AddAlias("u");
		}
		
		// remove subcommand
		{
			var idArgument = new Argument<long[]>("id")
			{
				Arity = ArgumentArity.OneOrMore,
			};
			Remove = new(
				name: "remove")
			{
				idArgument
			};
			Remove.SetHandler((ids) =>
			{
				foreach (var id in ids)
				{
					var entity = _dbSet.GetById(id);
					if (entity is not null) _dbSet.Remove(entity);
				}
			}, idArgument);
			foreach (var alias in (string[])[ "r", "delete", "d" ])
				Remove.AddAlias(alias);
		}
		
		// main command
		Command = new("series")
		{
			Query,
			Add,
			Update,
			Remove,
		};
	}
}
