using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.DB;

public static class DbSetExtensions
{
	public static TEntity? GetById<TEntity, TId>(this DbSet<TEntity> set, TId id)
		where TEntity : class, IIndetifiable<TId> => set
			.Where(entity => entity.Id != null && entity.Id.Equals(id))
			.First();
	public static IEnumerable<TEntity> GetByIds<TEntity, TId>(this DbSet<TEntity> set, IEnumerable<TId> ids)
		where TEntity : class, IIndetifiable<TId> => set
			.Where(entity => entity.Id != null && ids.Any(id => entity.Id.Equals(id)));
}