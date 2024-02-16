using System.Diagnostics;
using System.Threading.Tasks;
using MalApi;
using Spectre.Console;

static class AuthUtil {
	public static string GetMalClientId() => AnsiConsole.Ask<string>("[dodgerblue1]MAL client ID[/]:");
	public async static Task<string> GetMalToken(string malClientId) => await AnsiConsole.Status()
		.StartAsync("Preparing HTTP Listener...", async ctx =>
		{
			using var httpListener = new System.Net.HttpListener()
			{
				Prefixes = { "http://localhost:5567/auth/" }
			};

			Logger.Debug("Starting HTTP listener...");

			httpListener.Start();

			Logger.Debug("Started HTTP listener");
			
			ctx.Status("Waiting for authentication code...");
			Logger.Debug("Opening browser...");

			string url = MalAuthHelper.GetAuthUrl(malClientId);
			Logger.Trace($"Using [link{url}]URL[/]: {url}");

			Process.Start(new ProcessStartInfo
			{
				FileName = url,
				UseShellExecute = true
			});
			var context = await httpListener.GetContextAsync();
			var code = context.Request.QueryString["code"];

			Logger.Debug("Received authentication code");
			Logger.Trace(code);

			var buffer = System.Text.Encoding.UTF8.GetBytes(@"You may now close this tab.");
			context.Response.ContentLength64 = buffer.Length;
			context.Response.OutputStream.Write(buffer, 0, buffer.Length);
			context.Response.OutputStream.Close();

			ctx.Status($"Authenticating with MAL...");
			Logger.Debug("Requesting access token...");

			var token = await MalAuthHelper.DoAuth(malClientId, code);
			Logger.Debug("Access token received");
			Logger.Trace(token.AccessToken);
			Logger.Info("Authenticated successfully!");

			return token.AccessToken;
		});
	public static void SetAuth(this MalClient malClient, MyAnimeListConfig myAnimeList) {
		var (clientId, accessToken) = myAnimeList;
		Logger.Trace($"Client ID: {clientId}");
		Logger.Trace($"Access Token: {accessToken}");
		malClient.SetClientId(clientId);
		malClient.SetAccessToken(accessToken);
	}
}