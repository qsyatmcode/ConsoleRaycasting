using System.Text;

namespace ConsoleRaycasting;

using static Logger;

internal struct Map
{
	public char[,] Tiles { get; init; }
	public ushort XMax { get; init; } = 0;
	public ushort YMax { get; init; } = 0;
	private string _stringRepresentation = string.Empty;
	public Map(string mapFile)
	{
		using (StreamReader sr = new StreamReader(mapFile))
		{
			string map = sr.ReadToEnd();
			if (map == "")
				throw new ArgumentNullException("map file is empty");

			string[] lines = map.Split('\n');
			if (lines.Length < 3)
				throw new ArgumentNullException("map is invalid");
			YMax = (ushort)lines.Length;
			Log($"Map YMax = {YMax}"); // DEBUG
			XMax = (ushort)(lines[0].Length - 1); // с учётом символа переноса строки
			Log($"Map XMax = {XMax}"); // DEBUG

			Tiles = new char[YMax, XMax];
			
			//ushort x = 0, y = 0;
			for(ushort y = 0; y < YMax; y++)
			{
				for(ushort x = 0; x < XMax; x++)
				{
					Tiles[y, x] = lines[y][x];
				}
			}
		}
		
		Log("Map is successfully created with properties:");
		Log($"BufferHeight: {Console.BufferHeight}");
		Log($"BufferWidth: {Console.BufferWidth}");
		
		//PrintAtPosition(10, 5); // DEBUG
	}

	public void PrintAtPosition(int xpos, int ypos)
	{
		var lines = this.ToString().Split(Environment.NewLine);
		
		for (ushort i = 0; i < lines.Length; i++)
		{
			Console.SetCursorPosition(xpos, ypos + i);
			Console.WriteLine(lines[i]);
		}
		
		Console.SetCursorPosition(0, 0);
	}

	public char this[int y, int x] => Tiles[y, x];

	public override string ToString()
	{
		if (_stringRepresentation == string.Empty)
		{
			var result = new StringBuilder();

			for (int y = 0; y < YMax; y++)
			{
				for (int x = 0; x < XMax; x++)
				{
					result.Append(Tiles[y, x]);
				}

				if (y != (YMax - 1))
					result.Append(Environment.NewLine);
			}

			_stringRepresentation = result.ToString();
			return _stringRepresentation;
		}
		else
		{
			return _stringRepresentation;
		}
	}
}