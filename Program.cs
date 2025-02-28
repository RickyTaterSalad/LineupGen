// See https://aka.ms/new-console-template for more information
using GameGenerator;
using System.Text.RegularExpressions;
using System.Xml;


Console.WriteLine("1 - Current Lineup (TEXT FILE)");
Console.WriteLine("2 - Archive Existing");
Console.WriteLine("3 - Current Lineup (JOPLIN TABLE)");
var cmd = string.Empty;
var retries = 10;
var count = 0;

while (cmd != "1" && cmd != "2" && cmd != "3" && count++ < retries)
{
	cmd  = Console.ReadLine();
}
count = 0;
if (cmd == "3")
{
	var joplinTextPath = string.Empty;
	while (!File.Exists(joplinTextPath) && count++ < retries)
	{
		Console.WriteLine("Path To Joplin Text:");
		joplinTextPath = Console.ReadLine();
	}
	if (File.Exists(joplinTextPath))
	{
		var tableRowRegexp = new Regex("\\|[0-9\\s]+\\| (.*) \\| (.*) \\|");
		var title = string.Empty;
		var joplinLines = File.ReadAllLines(joplinTextPath);
		int curr = 0;
		var lineupDict = new Dictionary<string, Dictionary<string, string>>();
		var currentDict = string.Empty;

		foreach (var line in joplinLines)
		{
			if (curr == 0)
			{
				title = line[1..].Trim();
			}
			else
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}
				if (line.StartsWith("#"))
				{
					currentDict = line[1..].Trim();
					if (!lineupDict.ContainsKey(currentDict))
					{
						lineupDict.Add(currentDict, new Dictionary<string, string>());
					}
				}
				else
				{
					//some sort of table entry
					if (line.Contains("---"))
					{
						continue;
					}
					if (line.Contains("Order") && line.Contains("Name") && line.Contains("Position"))
					{
						continue;
					}
					else
					{
						var m = tableRowRegexp.Match(line);
						if (m.Success && m.Groups.Count > 2)
						{
							var player = m.Groups[1].Value.Trim();
							var position = m.Groups[2].Value.Trim();
							if (lineupDict.ContainsKey(currentDict))
							{
								lineupDict[currentDict].Add(player, position);

							}
						}
					}

				}
			}
			curr++;
		}
		//write temp file
		var lines = new List<string>()
		{
			$"!{title}"
		};
		foreach (var kvp in lineupDict)
		{
			lines.Add($"#{kvp.Key}");
			foreach (var playersKvp in kvp.Value)
			{
				lines.Add($"{playersKvp.Key};{playersKvp.Value}");
			}
		}
		var tempFile = Path.GetTempFileName();
		File.WriteAllLines(tempFile, lines);
		Parser.WriteLineupTable(tempFile, Path.GetDirectoryName(joplinTextPath) ?? string.Empty);
		File.Delete(tempFile);
	}
}
else if (cmd == "1")
{
	var lineupTextPath = string.Empty;
	while (!File.Exists(lineupTextPath) && count++ < retries)
	{
		Console.WriteLine("Path To Lineup Text:");
		lineupTextPath = Console.ReadLine();
	}
	if (File.Exists(lineupTextPath))
	{
		Parser.WriteLineupTable(lineupTextPath, Path.GetDirectoryName(lineupTextPath) ?? string.Empty);
	}
}

else if (cmd == "2")
{
	var userProvidedPath = string.Empty;
	while (!File.Exists(userProvidedPath) && count++ < retries)
	{
		Console.WriteLine("Path To Existing Lineup HTML:");
		userProvidedPath = Console.ReadLine();
	}
	var gameTitle = string.Empty;
	if (File.Exists(userProvidedPath))
	{
		var allText = File.ReadAllText(userProvidedPath);
		var titleRegexp = new Regex("<title>(.*)<\\/title>");
		var title = string.Empty;
		var m = titleRegexp.Match(allText);
		if (m.Success && m.Groups.Count > 1)
		{
			title = m.Groups[1].Value;
		}
		allText = allText.Replace("/archive/", "/").Replace("../style.css","../../style.css");

		count = 0;
		userProvidedPath = string.Empty;
		while (!Directory.Exists(userProvidedPath) && count++ < retries)
		{
			Console.WriteLine("Output Directory:");
			userProvidedPath = Console.ReadLine();
		}
		if (Directory.Exists(userProvidedPath))
		{
			var outputFile = Path.Combine(userProvidedPath, title, ".html");
			File.WriteAllText(outputFile, allText);
		}
	}
}
