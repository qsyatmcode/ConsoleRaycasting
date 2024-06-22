using ConsoleRaycasting.Misc;

namespace ConsoleRaycasting;

using static Logger;

class Program
{
	private const string ConfigFile = "config.txt";
	private const string LogFolder = "";
	private const UInt16 LogBufferSize = 2;
	
	static void Main(string[] args)
	{
		Logger.Setup(LogLevel.ALL, LogBufferSize);

		Config? config = null;
		try
		{
			config = new Config(ConfigFile);
		}
		catch (FileNotFoundException e)
		{
			Log("The config file could not be founded", LogLevel.ERROR);
			config = null;
		}
		catch
		{
			Log("There were problems reading config file", LogLevel.ERROR);
			config = null;
		}
		Log("Config successfully loaded");

		if (config == null)
			return;
		
		Game game = Game.Init(config);

		Log("Press any key to start");
		//Console.ReadKey();
		game.Start(); // starts game loop
		
	}
}