using Microsoft.EntityFrameworkCore;

namespace AnimeListSync.DB;

public static class DbSetExtensions
{
	public static TEntity? GetById<TEntity, TId>(this DbSet<TEntity> set, TId id)
		where TEntity : class, IIndetifiable<TId> => set
			.Where(entity => entity != null && entity.Id != null && entity.Id.Equals(id))
			.First();
}