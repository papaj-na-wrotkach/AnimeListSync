using System.ComponentModel.DataAnnotations;
using System.IO;
using AnimeListSync;
using Microsoft.EntityFrameworkCore;

[Index(nameof(ProviderId), IsUnique = true)]
public abstract class SeriesBase
{
	[Key]
	public abstract long Id { get; set; }
	public abstract long ProviderId { get; set; }
	public abstract ListProvider Provider { get; set; }
	public static void Deconstruct(SeriesBase series, out long id, out long providerId, out ListProvider provider)
	{
		id = series.Id;
		providerId = series.ProviderId;
		provider = series.Provider;
	}
}

public class MalSeries : SeriesBase
{
	public override long Id { get; set; }
	public override long ProviderId { get; set; }

	public override ListProvider Provider { get; set; } = ListProvider.MyAnimeList;
}

public class ShindenSeries : SeriesBase
{
	public override long Id { get; set; }
	public override long ProviderId { get; set; }
	public override ListProvider Provider { get; set; } = ListProvider.Shinden;
}

public class Association
{
	[Key]
	public long Id { get; set; }
	public static void Deconstruct(Association association, out long id)
	{
		id = association.Id;
	}
}

public class SeriesAssociation
{
	public long AssociationId { get; set; }
	public long AnimeId { get; set; }
	public static void Deconstruct(SeriesAssociation seriesAssociation, out long associationId, out long animeId)
	{
		associationId = seriesAssociation.AssociationId;
		animeId = seriesAssociation.AnimeId;
	}
}

public class Database : DbContext
{
	public DbSet<SeriesBase> Series { get; set; } = null!;
	public DbSet<Association> Associations { get; set; } = null!;
	public DbSet<SeriesAssociation> SeriesAssociations { get; set; } = null!;
	public string DbPath { get; init; } = Path.Join(Constants.configDir, "AnimeListSync.db");
	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}");
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configure the TPH (Table-Per-Hierarchy) mapping for AnimeBase and its derived classes
		modelBuilder.Entity<SeriesBase>()
			.HasDiscriminator<ListProvider>("Provider")
			.HasValue<MalSeries>(ListProvider.MyAnimeList)
			.HasValue<ShindenSeries>(ListProvider.Shinden);

		modelBuilder.Entity<SeriesBase>()
			.HasIndex(a => a.ProviderId)
			.IsUnique();

		modelBuilder.Entity<SeriesAssociation>()
			.HasKey(sa => new { sa.AnimeId, sa.AssociationId });

		base.OnModelCreating(modelBuilder);
	}

}