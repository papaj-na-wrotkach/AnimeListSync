using AnimeListSync.DB.Entity;
using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.DB;

public class Database : DbContext
{
	public DbSet<ProviderEntity> ProviderSet { get; set; } = null!;
	
	public DbSet<InternalSeriesEntity> InternalSeriesSet { get; set; } = null!;
	
	public DbSet<ProviderSeriesEntity> ProviderSeriesSet { get; set; } = null!;

	public required string DbPath { get; init; }

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}"); 
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ProviderEntity>()
			.HasData(Enum.GetValues<Provider>().Select(p => (ProviderEntity)p));
		
		modelBuilder.Entity<ProviderSeriesEntity>()
			.HasKey(a => new { a.ProviderId, a.IdFromProvider });
		
		base.OnModelCreating(modelBuilder);
	}
}