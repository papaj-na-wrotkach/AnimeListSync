using System.CommandLine;
using System.CommandLine.Binding;
using System.Dynamic;

namespace AnimeListSync.CLI.Commands;

public class InternalSeriesUpdateBinder : BinderBase<dynamic>
{
	public required Option<long?> IdOption { private get; init; }
	public required Option<string?> NameOption { private get; init; }
	public required Option<string?> DescriptionOption { private get; init; }

	protected override dynamic GetBoundValue(BindingContext ctx)
	{
		dynamic updateDto = new ExpandoObject();

		var id = ctx.ParseResult.GetValueForOption(IdOption);
		if (id is not null) updateDto.Id = id;
		var name = ctx.ParseResult.GetValueForOption(NameOption);
		if (name is not null) updateDto.Name = name;
		var description = ctx.ParseResult.GetValueForOption(DescriptionOption);
		if (description is not null) updateDto.Description = description;

		return updateDto;
	}
}