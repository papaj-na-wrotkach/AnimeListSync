namespace AnimeListSync.DB;

public interface IIndetifiable<TId>
{
	public abstract TId Id { get; }
}