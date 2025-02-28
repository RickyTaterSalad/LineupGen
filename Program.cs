using GameGenerator;
using System.Text.RegularExpressions;

Console.WriteLine("1 - Current Lineup (JOPLIN TABLE)");
Console.WriteLine("2 - Archive Current");
var cmd = string.Empty;
var retries = 10;
var count = 0;

while (cmd != "1" && cmd != "2" && cmd != "3" && count++ < retries)
{
	cmd  = Console.ReadLine();
}
count = 0;
if (cmd == "1")
{
	var joplinTextPath = string.Empty;
	while (!File.Exists(joplinTextPath) && count++ < retries)
	{
		Console.WriteLine("Path To Joplin Text:");
		joplinTextPath = Console.ReadLine();
	}
	if (File.Exists(joplinTextPath))
	{
		var tableRowRegexp = new Regex("\\|[0-9\\s]+\\|(.*)\\|(.*)\\|(.*)\\|");
		var title = string.Empty;
		var joplinLines = File.ReadAllLines(joplinTextPath);
		int curr = 0;
		var lineupDict = new Dictionary<string, List<Tuple<string,string, string>>>();
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
						lineupDict.Add(currentDict,new List<Tuple<string, string, string>>());
					}
				}
				else
				{
					//some sort of table entry
					if (line.Contains("---"))
					{
						continue;
					}
					if (line.Contains("Order") && line.Contains("Name") && line.Contains("Number") &&  line.Contains("Position"))
					{
						continue;
					}
					else
					{
						var m = tableRowRegexp.Match(line);
						if (m.Success && m.Groups.Count > 3)
						{
							var player = m.Groups[1].Value.Trim();
							var number = m.Groups[2].Value.Trim();
							var position = m.Groups[3].Value.Trim();
							if (lineupDict.ContainsKey(currentDict))
							{
								lineupDict[currentDict].Add(new Tuple<string,string,string>(player, number, position));

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
			foreach (var playerTuple in kvp.Value)
			{
				lines.Add($"{playerTuple.Item1};{playerTuple.Item2};{playerTuple.Item3}");
			}
		}
		var tempFile = Path.GetTempFileName();
		File.WriteAllLines(tempFile, lines);
		Parser.WriteLineupTable(tempFile, Path.GetDirectoryName(joplinTextPath) ?? string.Empty);
		File.Delete(tempFile);
	}
}

else if (cmd == "2")
{
	var userProvidedPath = string.Empty;
	while (!File.Exists(userProvidedPath) && count++ < retries)
	{
		Console.WriteLine(@"Path To Existing Lineup HTML: (Default: C:\Github\BaseballWebsite\2025\Mustang\Cubs\index.html)");
		userProvidedPath = Console.ReadLine();
		if(string.IsNullOrWhiteSpace(userProvidedPath)){
			userProvidedPath = @"C:\Github\BaseballWebsite\2025\Mustang\Cubs\index.html";
		}
	}
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
		var archiveFolder = Path.Combine(Path.GetDirectoryName(userProvidedPath) ?? string.Empty, "archive");
		var archiveIndex = Path.Combine(archiveFolder,"index.html");
		if(!Directory.Exists(archiveFolder)){
			Directory.CreateDirectory(archiveFolder);
		}
		if (Directory.Exists(archiveFolder))
		{
			var outputFile = Path.Combine(archiveFolder, $"{title}.html");
			File.WriteAllText(outputFile, allText);
			if(File.Exists(archiveIndex)){
				Parser.UpdateArchiveHtml(archiveIndex, outputFile);
			}
		}
	}
}
