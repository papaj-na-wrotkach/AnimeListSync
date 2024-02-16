using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnimeListSync.DB.Entity;

public class ProviderSeriesEntity : IIndetifiable<long>
{
	[Key]
	public long Id { get; set; }

	[NotNull, JsonIgnore]
	public InternalSeriesEntity InternalSeries { get; set; } = null!;
	[ForeignKey(nameof(InternalSeries))]
	public long InternalSeriesId { get; set; }

	[NotNull, JsonIgnore]
	public ProviderEntity Provider { get; set; } = null!;
	[ForeignKey(nameof(Provider))]
	public byte ProviderId { get; set; }

	[NotNull]
	public string IdFromProvider { get; set; } = null!;

	public override string ToString() => JsonSerializer.Serialize(this);
}