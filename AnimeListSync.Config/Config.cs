using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AnimeListSync.Config;

public class ConfigInstance {
	public MyAnimeListConfig MyAnimeList { get; set; } = new();

	private static readonly IDeserializer _deserializer = new DeserializerBuilder()
		.WithNamingConvention(PascalCaseNamingConvention.Instance)
		.IgnoreUnmatchedProperties()
		.Build();

	private static readonly ISerializer _serializer = new SerializerBuilder()
		.WithNamingConvention(PascalCaseNamingConvention.Instance)
		.Build();

	public static ConfigInstance FromFile(string? path = null) {
		_ = Directory.CreateDirectory((path is null ? null : new FileInfo(path).Directory?.FullName) ?? Constants.configDir);
		try
		{
			return _deserializer.Deserialize<ConfigInstance>(File.ReadAllText(path ?? Constants.configPath));
		}
		catch (FileNotFoundException)
		{
			return new();
		}
	}

	public static void Save(ConfigInstance config, string? path = null)
	{
		_ = Directory.CreateDirectory((path is null ? null : new FileInfo(path).Directory?.FullName) ?? Constants.configDir);
		File.WriteAllText(path ?? Constants.configPath, _serializer.Serialize(config));
	}
	
	public void Save() => Save(this);
	
	public void Deconstruct(out MyAnimeListConfig myAnimeList) {
		myAnimeList = MyAnimeList;
	}
}

public class MyAnimeListConfig {
	public const string MyAnimeList = nameof(MyAnimeList);
	public string ClientId { get; set; } = null!;
	public string AccessToken { get; set; } = null!;

	public void Deconstruct(out string clientId, out string accessToken) {
		clientId = ClientId;
		accessToken = AccessToken;
	}

}
