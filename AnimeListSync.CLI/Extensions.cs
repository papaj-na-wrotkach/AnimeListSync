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
	public static void Add(this Command thiz, IEnumerable<Argument> arguments)
	 => AddArgumentRange(thiz, arguments);
	public static void Add(this Command thiz, IEnumerable<Option> options)
		=> AddOptionRange(thiz, options);
	public static void Add(this Command thiz, IEnumerable<Command> commands)
		=> AddCommandRange(thiz, commands);
}