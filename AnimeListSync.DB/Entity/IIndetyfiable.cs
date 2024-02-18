namespace AnimeListSync.DB;

public interface IIndetifiable<TId>
{
	public TId Id { get; set; }
}