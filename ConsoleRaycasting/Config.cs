using System.Collections;

namespace ConsoleRaycasting
{
	namespace Misc
	{
		internal sealed class Config : IEnumerable
		{
			private const UInt16 ConfigLineMinLength = 3;
			private const UInt16 ConfigKeyMinLength = 2;
			private const char CommentsChar = ';';
			public Dictionary<string, string> Values { get; private set; }

			public Config(string path)
			{
				Values = ReadConfigFile(path);
			}
			
			private Dictionary<string, string> ReadConfigFile(string path)
			{
				Dictionary<string, string> results = new Dictionary<string, string>();
				string[] lines = File.ReadAllLines(path);
				string key, value;
				bool keyReading = true;
				
				foreach (var line in lines)
				{
					if (line.Length < ConfigLineMinLength || line is [CommentsChar, ..])
						continue;

					string[] parts = line.Split(' ');
					if (parts.Length < 3)
						continue;

					List<string> trimmed = Trim(parts); // Будет удалять всё, чья длина меньше ConfigKeyMinLength

					if (trimmed.Count < 3)
						continue;

					var pair = GetKeyValue(trimmed, out bool success);
					if(!success) 
						continue;
					else
						results.Add(pair.Key, pair.Value);
				}

				return results;
			}

			private List<string> Trim(string[] s)
			{
				var strs = s.ToList();
				
				for (int i = 0; i < strs.Count; i++)
				{
					string trimmed = strs[i].Trim();
					if (trimmed.Length < ConfigKeyMinLength)
					{
						if (trimmed is not ['='])
						{
							strs.RemoveAt(i);
							continue;
						}
					} 
					strs[i] = trimmed;
				}

				return strs;
			}

			private (string Key, string Value) GetKeyValue(List<string> parts, out bool success)
			{
				success = false;
				(string Key, string Value) results = (string.Empty, string.Empty);
				UInt16 keyIndex = 0;
				foreach (var part in parts)
				{
					keyIndex++;
					if (part.Length > 0)
					{
						results.Key = part;
						break;
					}
				}

				bool isKey = false;
				foreach (var part in parts[keyIndex..])
				{
					if (part is ['='])
					{
						isKey = true;
					}
					else
					{
						if (isKey)
						{
							if (part.Length > 0)
								results.Value = part;
						}
					}
				}

				if (results.Key != string.Empty && results.Value != string.Empty)
					success = true;
				else 
					success = false;

				return results;
			}

			public IEnumerator GetEnumerator() => Values.GetEnumerator();
		}
	}
}