using System.Diagnostics;
using ConsoleRaycasting.Misc;

namespace ConsoleRaycasting;

using static Logger;

internal class Game
{
	private readonly Dictionary<string, float>? _gameConfig = null;
	private Map Map { get; init; }
	private readonly int _screenWidth;
	private readonly int _screenHeight;
    
	private static Game? _instance = null;
	
    private Game(Config cfg)
    {
	    _screenHeight = Console.BufferHeight;
	    _screenWidth = Console.BufferWidth;
	    _gameConfig = new Dictionary<string, float>() // Initialize with default values
	    {
		    { "MoveSpeed", 30.0f },
		    { "RotatingSpeed", 15.0f },
		    { "DepthOfField", 16.0f}
	    };
	    
	    if(cfg.Values.Count > 0)
			ConfigApply(cfg);

	    if (cfg.Values.Count > 0)
	    {
		    if (cfg.Values.TryGetValue("MapFile", out var value))
		    {
			    this.Map = new Map(value);
		    }
		    else
		    {
			    throw new ArgumentException("ГДЕ КАРТА??");
		    }
	    }

	    Player player = new Player(_gameConfig["MoveSpeed"]);
        
		Log("Loaded Game Config params:");
		foreach (var param in _gameConfig)
		{
			Log($"{param.Key} - {param.Value}");
		}
	}
	
    public static Game Init(Config cfg)
    {
	    Console.CursorVisible = false;
	    
	    if (_instance != null)
	    {
		    Log("Game is already initialized!", LogLevel.WARNING);
		    return _instance;
	    }

        
		Log("Game inits..");
		
	    Game game = new Game(cfg);
	    _instance = game;
	    
		Log("Game is successfully initialized");

		return game;
    }

    char GetShadedChar(float shade)
    {
	    if (shade > 0.8f) return '#';
	    if (shade > 0.6f) return '=';
	    if (shade > 0.4f) return '-';
	    if (shade > 0.2f) return '.';
	    return ' ';
    }
    
    public void Start()
    {
	    char[] screen = new char[_screenWidth * _screenHeight];
	    
	    float depth = _gameConfig["DepthOfField"];
		float playerA = 0.0f;
		float fov = MathF.PI / 3.0f;

		float playerX = 4.0f;
		float playerY = 4.0f;

		float moveSpeed = _gameConfig["MoveSpeed"];
		float rotatingSpeed = _gameConfig["RotatingSpeed"];
		

		TimeOnly startTime = TimeOnly.FromDateTime(DateTime.Now);
		
		while (true)
		{
			TimeOnly endTime = TimeOnly.FromDateTime(DateTime.Now);
			float deltaTime = (float)(endTime - startTime).TotalSeconds;
			startTime = TimeOnly.FromDateTime(DateTime.Now);
			
			if (Console.KeyAvailable)
			{
				var consoleKey = Console.ReadKey(true).Key;

				switch (consoleKey)
				{
					case ConsoleKey.E:
						playerA -= deltaTime * rotatingSpeed;
						break;
					case ConsoleKey.Q:
						playerA += deltaTime * rotatingSpeed;
						break;
					case ConsoleKey.W:
						playerX += MathF.Cos(playerA) * moveSpeed * deltaTime;
						playerY += MathF.Sin(playerA) * moveSpeed * deltaTime;
						break;
					case ConsoleKey.S:
						playerX -= MathF.Cos(playerA) * moveSpeed * deltaTime;
						playerY -= MathF.Sin(playerA) * moveSpeed * deltaTime;
						break;
					case ConsoleKey.D:
						playerY -= MathF.Cos(playerA) * moveSpeed * deltaTime;
						break;
					case ConsoleKey.A:
						playerY += MathF.Cos(playerA) * moveSpeed * deltaTime;
						break;
				}
			}
		    for (int r = 0; r < _screenWidth; r++) // Пускаем лучи
		    {
			    float distanceToWall = 0.0f;

			    float ray = playerA + (fov / 2.0f) - ((r * fov) / _screenWidth);

			    float rayY = MathF.Sin(ray);
			    float rayX = MathF.Cos(ray);

			    bool isHitten = false;

			    while (!isHitten && distanceToWall <= depth)
			    {
				    distanceToWall += 0.1f;

				    int tileX = (int)(playerX + distanceToWall * rayX);
				    int tileY = (int)(playerY + distanceToWall * rayY);

				    if (distanceToWall < 0 || tileX >= playerX + depth || tileY >= playerY + depth)
				    {
					    isHitten = true;
					    distanceToWall = depth;
				    }
				    else
				    {
					    char tile = Map[tileY, tileX];

					    if (tile == '#') // Луч упёрся в стену
					    {
						    isHitten = true;
					    }
				    }
			    }

			    char wallShade;
			    if (distanceToWall <= depth / 4f)
				    wallShade = '\u2588';
			    else if (distanceToWall < depth / 3f)
				    wallShade = '\u2593';
			    else if (distanceToWall < depth / 2f)
				    wallShade = '\u2592';
			    else if (distanceToWall < depth)
				    wallShade = '\u2591';
			    else
				    wallShade = ' ';

				
				float correctedDistance = distanceToWall * MathF.Cos(ray - playerA);

				int ceiling = (int)(_screenHeight / 2f - _screenHeight / correctedDistance /*distanceToWall*/);
			    int floor = _screenHeight - ceiling;

			    for (int sh = 0; sh < _screenHeight; sh++)
			    {
				    if (sh <= ceiling) // небо
				    {
					    screen[sh * _screenWidth + r] = ' ';
				    }
				    else if (sh > ceiling && sh <= floor ) // стена
				    {
					    screen[sh * _screenWidth + r] = wallShade;
				    }
				    else // ПОЛ
				    {
					    screen[sh * _screenWidth + r] = 'x';
				    }
			    }
		    }

		    Console.SetCursorPosition(0, 0);
		    Console.Write(screen);
	    }
    }
    
	private void ConfigApply(Config cfg)
	{
		foreach (var param in _gameConfig)
		{
			foreach (var value in cfg.Values)
			{
				if (param.Key == value.Key)
				{
					float cfg_value;
					if (float.TryParse(value.Value, out cfg_value))
					{
						_gameConfig[param.Key] = cfg_value;
					}

					cfg.Values.Remove(value.Key);
				}
			}
		}
	}
}