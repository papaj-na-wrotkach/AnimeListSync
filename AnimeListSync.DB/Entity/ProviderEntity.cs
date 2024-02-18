using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace AnimeListSync.DB.Entity;

public enum Provider : byte
{
	[Description("MyAnimeList (https://myanimelist.net)")]
	MyAnimeList,
	[Description("Shinden (https://shinden.pl)")]
	Shinden
}

public class ProviderEntity : IIndetifiable<byte>
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public byte Id { get; set; }
	
	[NotNull]
	public string Name { get; set; } = null!;
	
	public string? Description { get; set; }

	protected ProviderEntity() { }

	private ProviderEntity(Provider @enum)
	{
		Id = (byte)@enum;
		Name = @enum.ToString();
		Description = @enum
			.GetType()
			.GetField(@enum.ToString())?
			.GetCustomAttributes(typeof(DescriptionAttribute), false)
			.Cast<DescriptionAttribute>()
			.FirstOrDefault()?.Description ?? string.Empty;
	}
	
	public static implicit operator ProviderEntity(Provider @enum) => new(@enum);

	public static implicit operator Provider(ProviderEntity provider) => (Provider)provider.Id;
}