using System.CommandLine;
using System.CommandLine.Binding;

namespace AnimeListSync.CLI.Commands;

public class InternalSeriesCreateBinder : BinderBase<dynamic>
{
	public required Argument<string> NameArgument { private get; init; }
	public required Argument<string?> DescriptionArgument { private get; init; }

	protected override dynamic GetBoundValue(BindingContext ctx) => new
	{
		Name = ctx.ParseResult.GetValueForArgument(NameArgument),
		Description = ctx.ParseResult.GetValueForArgument(DescriptionArgument) ?? string.Empty
	};
}