using System.Runtime.CompilerServices;

namespace ConsoleRaycasting;

internal enum LogLevel
{
	NONE,
	INFO,
	WARNING,
	ERROR,
	ALL,
}

internal static class Logger
{
	private static string? _logPath;
	private static LogLevel _logLevel;
	private static string[]? _logBuffer;
	private static bool _initialized = false;

	public static void Setup(LogLevel level, UInt16 bufferSize = 32, string path = "")
	{
		_logPath = path;
		_logLevel = level;
		_logBuffer = new string[bufferSize];
		_initialized = true;

		Log($"Logger has been setupped with the following parameters: Level - {level.ToString()}, Buffer size - {bufferSize}, Log file path - {Path.Combine(path, "log.txt")}");
	}

	private static int _bufferOccupancyCounter = 0;
    public static void Log(String message, LogLevel level = LogLevel.INFO)
	{
		if (!_initialized) return;
		if (_logLevel == LogLevel.NONE) return;
		
		Console.ResetColor();

		string output = level switch
		{ 
			LogLevel.INFO => LogInfo(message),
			LogLevel.WARNING => LogWarning(message),
			LogLevel.ERROR => LogError(message),
			_ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
		};
		if (level <= _logLevel)
		{
			Console.WriteLine(output);
			if (_bufferOccupancyCounter >= _logBuffer.Length)
			{ 
				using (StreamWriter sw = new StreamWriter(Path.Combine(_logPath, "log.txt"), true))
				{
					foreach (var log in _logBuffer)
					{
						sw.WriteLine(TimeOnly.FromDateTime(DateTime.Now) + " - " + log);
					}
				}
				Array.Clear(_logBuffer);
				_bufferOccupancyCounter = 0;
			}

			_logBuffer[_bufferOccupancyCounter] = output;
			_bufferOccupancyCounter++;
		}
		
		Console.ResetColor();
	}

	private static String LogInfo(string message) => $"[INFO] {message};";

	private static String LogWarning(string message)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		return $"[WARNING] {message};";
	}

	private static String LogError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		return $"[ERROR] {message};";
	}
}