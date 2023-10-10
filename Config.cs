using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Config {
	public void Save() => SaveConfig(this);
	public MyAnimeListConfig MyAnimeList { get; set; } = new();

	public void Deconstruct(out MyAnimeListConfig myAnimeList) {
		myAnimeList = MyAnimeList;
	}

	private static readonly IDeserializer _deserializer = new DeserializerBuilder()
		.WithNamingConvention(PascalCaseNamingConvention.Instance)
		.IgnoreUnmatchedProperties()
		.Build();
	private static readonly ISerializer _serializer = new SerializerBuilder()
		.WithNamingConvention(PascalCaseNamingConvention.Instance)
		.Build();

	public static Config FromFile(string? path = null) {
		_ = Directory.CreateDirectory((path is null ? null : new FileInfo(path).Directory?.FullName) ?? Constants.configDir);
		try
		{
			return _deserializer.Deserialize<Config>(File.ReadAllText(path ?? Constants.configPath));
		}
		catch (FileNotFoundException)
		{
			return new();
		}
	}
	public static void SaveConfig(Config config, string? path = null)
	{
		_ = Directory.CreateDirectory((path is null ? null : new FileInfo(path).Directory?.FullName) ?? Constants.configDir);
		File.WriteAllText(path ?? Constants.configPath, _serializer.Serialize(config));
	}
}

public class MyAnimeListConfig {
	public const string MyAnimeList = nameof(MyAnimeList);
	private string? clientId;
	public string ClientId { get => clientId ??= AuthUtil.GetMalClientId(); set => clientId = value; }
	private string? accessToken;
	public string AccessToken { get => accessToken ??= AuthUtil.GetMalToken(ClientId).Result; }

	public void Deconstruct(out string clientId, out string accessToken) {
		clientId = ClientId;
		accessToken = AccessToken;
	}

}
