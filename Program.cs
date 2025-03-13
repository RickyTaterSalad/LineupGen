using LineupGen;
using System.Text.RegularExpressions;
using static LineupGen.Options;

var opts = CommandLine.Parser.Default.ParseArguments<Options>(args)?.Value;
if(opts == null)
{
	Console.WriteLine("Invalid Options Passed");
	return;
}

var mode = opts.GetMode();
if(mode == MODE.NONE)
{
	Console.WriteLine("Invalid Mode Passed");
}

if (mode == MODE.PUBLISH)
{
	if (string.IsNullOrWhiteSpace(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory null");
		return;
	}
	if (!Directory.Exists(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory does not exist");
		return;
	}
	if (string.IsNullOrWhiteSpace(opts.MDDirectory))
	{
		Console.WriteLine("MDDirectory null");
		return;
	}
	if (!Directory.Exists(opts.MDDirectory))
	{
		Console.WriteLine("MDDirectory does not exist");
		return;
	}
	var teamIndexHTML = Path.Combine(opts.TeamRootDirectory, "index.html");

	var mdFile = new DirectoryInfo(opts.MDDirectory).GetFiles("*.md").OrderByDescending(f => f.LastWriteTime).FirstOrDefault()?.FullName;

	if (File.Exists(mdFile))
	{
		Console.WriteLine($"Using Table File: " + mdFile);
		var tableRowRegexp = new Regex("\\|[0-9\\s]+\\|(.*)\\|(.*)\\|(.*)\\|");
		var title = string.Empty;
		var joplinLines = File.ReadAllLines(mdFile);
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
		GameGenerator.Parser.WriteLineupTable(tempFile, teamIndexHTML);
		File.Delete(tempFile);
	}
}

else if (mode == MODE.ARCHIVE)
{
	if (string.IsNullOrWhiteSpace(opts.WebsiteRoot))
	{
		Console.WriteLine("WebsiteRoot null");
		return;
	}
	if (!Directory.Exists(opts.WebsiteRoot))
	{
		Console.WriteLine("WebsiteRoot does not exist");
		return;
	}
	if (string.IsNullOrWhiteSpace(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory null");
		return;
	}
		if (!Directory.Exists(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory does not exist");
		return;
	}
	var archiveHtmlPath = Path.Combine(opts.TeamRootDirectory,"index.html");
	if (!File.Exists(archiveHtmlPath))
	{
		Console.WriteLine($"{archiveHtmlPath} does not exist");
		return;
	}
	var templateFile = Path.Combine(opts.WebsiteRoot, "templates", "archive", "empty_lineup.html");
	GameGenerator.Parser.ArchiveHtmlFile(archiveHtmlPath, templateFile);
}

else if (mode == MODE.YOUTUBE)
{
	//set YouTube link on latest archived lineup
	if (string.IsNullOrWhiteSpace(opts.VideoUrl))
	{
		Console.WriteLine("VideoUrl null");
		return;
	}
	if (string.IsNullOrWhiteSpace(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory null");
		return;
	}
	if (!Directory.Exists(opts.TeamRootDirectory))
	{
		Console.WriteLine("TeamRootDirectory does not exist");
		return;
	}
	GameGenerator.Parser.UpdateLatestArchiveEntryWithYoutubeLink(opts.VideoUrl, opts.TeamRootDirectory ?? string.Empty);
}



//push the correct index.html
if (mode == MODE.ARCHIVE || mode == MODE.PUBLISH || mode == MODE.OFFLINE)
{
	if (string.IsNullOrWhiteSpace(opts.WebsiteRoot))
	{
		Console.WriteLine("WebsiteRoot null");
		return;
	}
	if (!Directory.Exists(opts.WebsiteRoot))
	{
		Console.WriteLine("WebsiteRoot does not exist");
		return;
	}
	Console.WriteLine("Replacing root index.html....");
	var templateFile = Path.Combine(opts.WebsiteRoot, "templates", mode == MODE.PUBLISH ? "index_lineup.html" : "index_no_lineup.html");
	Console.WriteLine($"Template: {templateFile}");
	if (File.Exists(templateFile))
	{
		Console.WriteLine($"Copying...");
		File.Copy(templateFile, Path.Combine(opts.WebsiteRoot, "index.html"), true);
		Console.WriteLine($"Copied.");
	}
}
