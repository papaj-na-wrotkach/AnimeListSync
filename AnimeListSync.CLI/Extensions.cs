using System.CommandLine;

namespace AnimeListSync.CLI;

public static class CommandExtensions
{
	public static void AddArgumentRange(this Command thiz, IEnumerable<Argument> arguments)
	{
		foreach (var argument in arguments) thiz.AddArgument(argument);
	}
	public static void AddOptionRange(this Command thiz, IEnumerable<Option> options)
	{
		foreach (var option in options) thiz.AddOption(option);
	}
	public static void AddCommandRange(this Command thiz, IEnumerable<Command> commands)
	{
		foreach (var command in commands) thiz.AddCommand(command);
	}
	public static void AddArgumentRange(this Command thiz, params Argument[] arguments)
		=> thiz.AddArgumentRange(arguments.AsEnumerable());
	public static void AddOptionRange(this Command thiz, params Option[] options)
		=> thiz.AddOptionRange(options.AsEnumerable());
	public static void AddCommandRange(this Command thiz, params Command[] commands)
		=> thiz.AddCommandRange(commands.AsEnumerable());
	public static void Add(this Command thiz, IEnumerable<Argument> arguments)
		=> thiz.AddArgumentRange(arguments);
	public static void Add(this Command thiz, IEnumerable<Option> options)
		=> thiz.AddOptionRange(options);
	public static void Add(this Command thiz, IEnumerable<Command> commands)
		=> thiz.AddCommandRange(commands);
	public static void Add(this Command thiz, params Argument[] arguments)
		=> thiz.AddArgumentRange(arguments);
	public static void Add(this Command thiz, params Option[] options)
		=> thiz.AddOptionRange(options);
	public static void Add(this Command thiz, params Command[] commands)
		=> thiz.AddCommandRange(commands);

	public static void AddAliasRange(this Command thiz, IEnumerable<string> aliases)
	{
		foreach (var alias in aliases) thiz.AddAlias(alias);
	}
	public static void AddAliasRange(this Command thiz, params string[] aliases)
		=> thiz.AddAliasRange(aliases.AsEnumerable());
	public static void AddAlias(this Command thiz, params string[] aliases)
		=> thiz.AddAliasRange(aliases);
}