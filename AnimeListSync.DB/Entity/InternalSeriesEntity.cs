using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AnimeListSync.DB.Entity;

public class InternalSeriesEntity : IIndetifiable<long>
{
	[Key]
	public long Id { get; set; } 
	
	[NotNull]
	public required string Name { get; set; }

	[NotNull]
	public required string Description { get; set; }
}
