using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using AnimeListSync;
using Microsoft.EntityFrameworkCore;

public class Database : DbContext
{
	public class Provider
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		protected Provider() { }

		private Provider(ListProvider @enum)
		{
			Id = (int)@enum;
			Name = @enum.ToString();
			Description = @enum
				.GetType()
				.GetField(@enum.ToString())?
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.Cast<DescriptionAttribute>()
				.FirstOrDefault()?.Description ?? string.Empty;
		}
		public static implicit operator Provider(ListProvider @enum) => new(@enum);

		public static implicit operator ListProvider(Provider provider) => (ListProvider)provider.Id;
	}

	public class DatabaseAnime
	{
		[Key]
		public long Id { get; set; }
	}

	public class ProviderAnime
	{
		[Key]
		public long Id { get; set; }
		public ListProvider Provider { get; set; }
	}

	public class AnimeAssociation
	{
		[ForeignKey(nameof(Database))]
		public long DatabaseId { get; set; }
		public DatabaseAnime Database { get; set; } = null!;

		[ForeignKey(nameof(Provider))]
		public long ProviderId { get; set; }
		public ProviderAnime Provider { get; set; } = null!;
	}

	public DbSet<Provider> Providers { get; set; } = null!;
	public DbSet<DatabaseAnime> DatabaseSeries { get; set; } = null!;
	public DbSet<ProviderAnime> ProviderSeries { get; set; } = null!;
	public DbSet<AnimeAssociation> Associations { get; set; } = null!;
	public string DbPath { get; init; } = Path.Join(Constants.configDir, "AnimeListSync.db");
	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={DbPath}");
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Provider>()
			.HasData(Enum.GetValues<ListProvider>().Select(p => (Provider)p));
		
		modelBuilder.Entity<AnimeAssociation>()
			.HasKey(a => new { a.DatabaseId, a.ProviderId });
		
		base.OnModelCreating(modelBuilder);
	}
}