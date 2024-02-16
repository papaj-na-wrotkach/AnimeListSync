using AnimeListSync.DB.Entity.Attribute;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace AnimeListSync.DB.Entity;

public class InternalSeriesEntity : IIndetifiable<long>
{
	[Key, CrudEditable(true)]
	public long Id { get; set; }
	
	[NotNull, CrudEditable(true)]
	public string Name { get; set; } = null!;

	[NotNull, CrudEditable(true)]
	public string Description { get; set; } = null!;

	// public override string ToString() => JsonSerializer.Serialize(this);
}
