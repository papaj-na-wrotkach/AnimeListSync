using System.Diagnostics;
using System.Threading.Tasks;
using MalApi;
using Spectre.Console;

static class AuthUtil {
	public static string GetMalClientId() => AnsiConsole.Ask<string>("[dodgerblue1]MAL client ID[/] not found! Please provide it here:");
	public async static Task<string> GetMalToken(string malClientId) => await AnsiConsole.Status()
		.StartAsync("Preparing HTTP Listener...", async ctx =>
		{
			using var httpListener = new System.Net.HttpListener()
			{
				Prefixes = { "http://localhost:5567/auth/" }
			};

			LogUtil.Verbose("Starting HTTP listener...");

			httpListener.Start();

			LogUtil.Verbose("Started HTTP listener");
			
			ctx.Status("Waiting for authentication code...");
			LogUtil.Verbose("Opening browser...");

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

			LogUtil.Verbose("Received authentication code");
			ctx.Status($"Authenticating with MAL...");
			LogUtil.Verbose("Requesting access token...");

			var token = await MalAuthHelper.DoAuth(malClientId, code);

			LogUtil.Verbose("Access token received");
			LogUtil.Info("Authenticated successfully!");
			
			return token.AccessToken;
		});
	public static void SetAuth(this MalClient malClient, MyAnimeListConfig myAnimeList) {
		var (clientId, accessToken) = myAnimeList;
		malClient.SetClientId(clientId);
		malClient.SetAccessToken(accessToken);
	}
}