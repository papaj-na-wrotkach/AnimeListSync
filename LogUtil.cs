using Spectre.Console;

public static class LogUtil {
	public static void Verbose(string message) => AnsiConsole.MarkupLine($"[gray]VERBOSE:[/]\t{message}");
	public static void Info(string message) => AnsiConsole.MarkupLine($"[blue]INFO:[/]\t{message}");
	public static void Warn(string message) => AnsiConsole.MarkupLine($"[orange]WARN:[/]\t{message}");
	public static void Error(string message) => AnsiConsole.MarkupLine($"[red]ERROR:[/]\t{message}");
	public static void Critical(string message) => AnsiConsole.MarkupLine($"[inverted red]CRIT:[/]\t{message}");
}