using Spectre.Console;
using System;

// Totally not ripped from Macrohard's code
public enum LogLevel
{
	//
	// Summary:
	//     Logs that contain the most detailed messages. These messages may contain sensitive
	//     application data. These messages are disabled by default and should never be
	//     enabled in a production environment.
	Trace,
	//
	// Summary:
	//     Logs that are used for interactive investigation during development. These logs
	//     should primarily contain information useful for debugging and have no long-term
	//     value.
	Debug,
	//
	// Summary:
	//     Logs that track the general flow of the application. These logs should have long-term
	//     value.
	Information,
	//
	// Summary:
	//     Logs that highlight an abnormal or unexpected event in the application flow,
	//     but do not otherwise cause the application execution to stop.
	Warning,
	//
	// Summary:
	//     Logs that highlight when the current flow of execution is stopped due to a failure.
	//     These should indicate a failure in the current activity, not an application-wide
	//     failure.
	Error,
	//
	// Summary:
	//     Logs that describe an unrecoverable application or system crash, or a catastrophic
	//     failure that requires immediate attention.
	Critical,
	//
	// Summary:
	//     Not used for writing log messages. Specifies that a logging category should not
	//     write any messages.
	None
}

public static class Logger {
	public static readonly LogLevel LogLevel = (LogLevel)int.Parse(Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "6");
	private static void Log(string? message, LogLevel level) {
		if (LogLevel > level || message is null) return;
		AnsiConsole.MarkupLine($"[{level switch {
			LogLevel.Trace => "grey]TRACE:",
			LogLevel.Debug => "blue]DEBUG:",
			LogLevel.Information => "green]INFO:",
			LogLevel.Warning => "olive]WARN:",
			LogLevel.Error => "red]ERROR:",
			LogLevel.Critical => "inverted red]CRIT:",
			_ => "default]",
		}}[/]\t{message}");
	}
	public static void Trace(string? message) => Log(message, LogLevel.Trace);
	public static void Debug(string? message) => Log(message, LogLevel.Debug);
	public static void Info(string? message) => Log(message, LogLevel.Information);
	public static void Warn(string? message) => Log(message, LogLevel.Warning);
	public static void Error(string? message) => Log(message, LogLevel.Error);
	public static void Critical(string? message) => Log(message, LogLevel.Critical);
}