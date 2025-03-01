using GameGenerator;
using System.Text.RegularExpressions;

Console.WriteLine("1 - Current Lineup (JOPLIN TABLE)");
Console.WriteLine("2 - Archive Current");
var cmd = string.Empty;
var retries = 10;
var count = 0;

const string defaultIndexHtml = "C:\\Github\\BaseballWebsite\\2025\\Mustang\\Cubs\\index.html";

var isSilentMode = false;

foreach (var arg in args){
	if(!isSilentMode && "silent".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		isSilentMode = true;
	}
}

if(!isSilentMode){
	while (cmd != "1" && cmd != "2" && cmd != "3" && count++ < retries)
	{
		cmd  = Console.ReadLine();
	}
}
else{
	//silent
	cmd = "1";
}
count = 0;
if (cmd == "1")
{
	var joplinTextPath = string.Empty;
	while (!File.Exists(joplinTextPath) && count++ < retries)
	{
		Console.WriteLine($"Path to .md table export directory (default: {Parser.defaultLineupProcessingFolder})");
		if(!isSilentMode){
			joplinTextPath = Console.ReadLine();
		}
		if (String.IsNullOrWhiteSpace(joplinTextPath))
		{
			joplinTextPath = Parser.defaultLineupProcessingFolder;
		}
		if (Directory.Exists(joplinTextPath))
		{
			joplinTextPath = new DirectoryInfo(joplinTextPath).GetFiles("*.md").OrderByDescending(f => f.LastWriteTime).FirstOrDefault()?.FullName;
		}
		if(isSilentMode){
			break;
		}
	}

	if (File.Exists(joplinTextPath))
	{
		var tableRowRegexp = new Regex("\\|[0-9\\s]+\\|(.*)\\|(.*)\\|(.*)\\|");
		var title = string.Empty;
		var joplinLines = File.ReadAllLines(joplinTextPath);
		int curr = 0;
		var lineupDict = new Dictionary<string, List<Tuple<string, string, string>>>();
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
						lineupDict.Add(currentDict, new List<Tuple<string, string, string>>());
					}
				}
				else
				{
					//some sort of table entry
					if (line.Contains("---"))
					{
						continue;
					}
					if (line.Contains("Order") && line.Contains("Name") && line.Contains("Number") && line.Contains("Position"))
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
								lineupDict[currentDict].Add(new Tuple<string, string, string>(player, number, position));

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
		Console.WriteLine($"Path To Existing Lineup HTML: (Default: {defaultIndexHtml})");
		var outputHTMLPath = string.Empty;
		if(!isSilentMode){
			outputHTMLPath = Console.ReadLine();
		}
		if (string.IsNullOrWhiteSpace(outputHTMLPath))
		{
			outputHTMLPath = defaultIndexHtml;
		}
		else
		{
			if (Directory.Exists(outputHTMLPath))
			{
				outputHTMLPath = Path.Combine(outputHTMLPath, "index.html");
			}
		}
		Parser.WriteLineupTable(tempFile, outputHTMLPath);
		File.Delete(tempFile);
	}
	Console.WriteLine("Complete... Press enter to exit.");
	Console.ReadKey();
}

else if (cmd == "2")
{
	var userProvidedPath = string.Empty;
	while (!File.Exists(userProvidedPath) && count++ < retries)
	{
		Console.WriteLine($"Path To Existing Lineup HTML: (Default: {defaultIndexHtml})");
		if(!isSilentMode){
			userProvidedPath = Console.ReadLine();
		}
		if (string.IsNullOrWhiteSpace(userProvidedPath))
		{
			userProvidedPath = defaultIndexHtml;
		}
		else
		{
			if (Directory.Exists(userProvidedPath))
			{
				userProvidedPath = Path.Combine(userProvidedPath, "index.html");
			}
		}
		if(isSilentMode){
			break;
		}
	}
	if (File.Exists(userProvidedPath))
	{
		Parser.ArchiveHtmlFile(userProvidedPath);
	}	
	Console.WriteLine("Complete... Press enter to exit.");
	Console.ReadKey();
}
