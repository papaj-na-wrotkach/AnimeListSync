using MalApi;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AnimeListSync;

public class Config
{
    public class MyAnimeListConfig {
        public const string MyAnimeList = nameof(MyAnimeList);
        public string? ClientId { get; set; }
        public string? AccessToken { get; set; }
    };
    public MyAnimeListConfig? MyAnimeList { get; set; } = new();    
}

public enum ListProvider
{
    MyAnimeList,
    Shinden
}

class Program
{
    private static readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "animelistsync");
    private static readonly string configFilePath = Path.Combine(configDir, "config.yml");
    
    private static ISerializer configSerializer { get; } = new YamlDotNet.Serialization.SerializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();

    private static IDeserializer configDeserializer { get; } = new YamlDotNet.Serialization.DeserializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance)
    .Build();

    private static Config ProgramConfig { get; set; } = new Config();

    private static void SaveConfig() {
        Directory.CreateDirectory(configDir);
        using var writer = File.CreateText(configFilePath);
        configSerializer.Serialize(writer, ProgramConfig);
    }

    private static MalClient malClient { get; } = new MalClient();

    static async Task<int> Main(params string[] args)
    {
        if (File.Exists(configFilePath)) ProgramConfig = configDeserializer.Deserialize<Config>(File.ReadAllText(configFilePath)) ?? ProgramConfig;
        malClient.SetClientId(ProgramConfig.MyAnimeList?.ClientId);
        malClient.SetAccessToken(ProgramConfig.MyAnimeList?.AccessToken);

        var providerOption = new Option<ListProvider>(
            name: "--provider",
            description: "List provider to use",
            getDefaultValue: () => ListProvider.MyAnimeList)
        {
            Arity = ArgumentArity.ExactlyOne,
            AllowMultipleArgumentsPerToken = false,
            IsRequired = false
        };
        providerOption.AddAlias("-p");

        var providerArgument = new Argument<ListProvider>(
            name: "provider",
            description: "List provider to use")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        var queryArgument = new Argument<List<string>>(
            name: "query",
            description: "Search query")
        {
            Arity = ArgumentArity.OneOrMore
        };

        var authCommand = new Command(
            name: "auth",
            description: "Authenticate with a list provider")
        {
            providerArgument
        };

        authCommand.SetHandler((ListProvider provider) =>
        {
            switch (provider)
            {
                case ListProvider.MyAnimeList:
                    malClient.SetClientId(ProgramConfig!.MyAnimeList!.ClientId ??= getMalClientId());
                    malClient.SetAccessToken(ProgramConfig.MyAnimeList.AccessToken = getMalAccessToken(ProgramConfig.MyAnimeList.ClientId!).Result);
                    break;
                case ListProvider.Shinden:
                    throw new NotImplementedException();
            }
            SaveConfig();
        }, providerArgument);

        var searchCommand = new Command(
            name: "search",
            description: "Search for a anime series")
        {
            queryArgument,
            providerOption
        };

        searchCommand.SetHandler(async (ListProvider provider, List<string> queries) =>
        {
            AnsiConsole.Prompt(new SelectionPrompt<Anime>()
            .AddChoices(provider switch
            {
                ListProvider.MyAnimeList => from search in (await Task.WhenAll(
                                                from query in queries
                                                select malClient.Anime().WithName(query).Find()))
                                            from series in search.Data
                                            select series,
                _ => throw new NotImplementedException()
            }));
        }, providerOption, queryArgument);

        var rootCommand = new RootCommand(
            description: "A fire-and-forget CLI app allowing users to sync their anime and manga lists between multiple providers")
        {
            authCommand,
            searchCommand
        };
        return await rootCommand.InvokeAsync(args);
    }

    private async static Task<string> getMalAccessToken(string malClientId) => await AnsiConsole.Status()
        .StartAsync("Preparing HTTP Listener...", async ctx =>
        {
            using var httpListener = new System.Net.HttpListener()
            {
                Prefixes = { "http://localhost:5567/auth/" }
            };
            AnsiConsole.MarkupLine($"[gray]LOG:[/] Starting HTTP listener...");
            httpListener.Start();
            AnsiConsole.MarkupLine($"[gray]LOG:[/] [green]Started HTTP listener[/]");
            
            ctx.Status("Waiting for authentication code...");
            AnsiConsole.MarkupLine($"[gray]LOG:[/] Opening browser...");
            Process.Start(new ProcessStartInfo
            {
                FileName = MalAuthHelper.GetAuthUrl(malClientId),
                UseShellExecute = true
            });
            var context = await httpListener.GetContextAsync();
            var code = context.Request.QueryString["code"];
            var buffer = System.Text.Encoding.UTF8.GetBytes(@"You may now close this tab.");
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            AnsiConsole.MarkupLine($"[gray]LOG:[/] [green]Received authentication code[/]");
            ctx.Status($"Authenticating with MAL...");
            AnsiConsole.MarkupLine($"[gray]LOG:[/] Requesting access token...");
            var token = await MalAuthHelper.DoAuth(malClientId, code);
            AnsiConsole.MarkupLine($"[gray]LOG:[/] [green]Access token received[/]");
            AnsiConsole.MarkupLine($"[green]Authenticated successfully![/]");
            return token.AccessToken;
        });

    private static string getMalClientId() => AnsiConsole.Ask<string>("[dodgerblue1]MAL client ID[/] not found! Please provide it here:");
}
