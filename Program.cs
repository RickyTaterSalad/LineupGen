using LineupGen;
using System.Text.RegularExpressions;
using static LineupGen.Options;

var opts = CommandLine.Parser.Default.ParseArguments<Options>(args)?.Value;
//MODE mode = MODE.TEST; //MODE.NONE;
MODE mode = MODE.NONE;
var testMode = mode == MODE.TEST;
if (testMode)
{
	opts ??= new Options();
	opts.Mode = "publish";
	opts.TeamRootDirectory = @"C:\Repos\BaseballWebsite\2025\Mustang\AllStars";
	opts.MDDirectory = @"C:\temp";
}
else if (opts == null)
{
	Console.WriteLine("Invalid Options Passed");
	return;
}
mode = opts.GetMode();

if (mode == MODE.NONE)
{
	Console.WriteLine("Invalid Mode Passed");
}

else if (mode == MODE.PUBLISH)
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
		var linupTableRowRegexp = new Regex("\\|[0-9\\s]+\\|(.*)\\|(.*)\\|(.*)\\|");
		var benchTableRowRegexp = new Regex("\\|(.*)\\|(.*)\\|");
		var title = string.Empty;
		var joplinLines = File.ReadAllLines(mdFile);
		int curr = 0;
		var lineupDict = new Dictionary<string, List<Tuple<string, string, string>>>();
		var benchList = new List<Tuple<string, string>>();
		var noteLines = new List<string>();
		var currentDict = string.Empty;
		var intoLineupNotes = false;
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
				if (line.StartsWith("#") && !line.StartsWith("##"))
				{
					currentDict = line[1..].Trim();
					if (!lineupDict.ContainsKey(currentDict))
					{
						lineupDict.Add(currentDict, new List<Tuple<string, string, string>>());
					}
				}
				else if (line.StartsWith("##") && line.Contains("Notes"))
				{
					intoLineupNotes = true;
					continue;
				}
				else
				{
					if (intoLineupNotes)
					{
						var trimmed = line.Trim();
						if (trimmed.Equals("substitutions", StringComparison.InvariantCultureIgnoreCase) || trimmed.Equals("pitching", StringComparison.InvariantCultureIgnoreCase))
						{
							noteLines.Add($"<br/><b>{trimmed}</b>");
							noteLines.Add($"<hr/>");
						}
						else {
							noteLines.Add(line);
						}
						continue;
					}
					//some sort of table entry
					if (line.Contains("---"))
					{
						continue;
					}
					if (line.Contains("Name") && line.Contains("Number"))
					{
						continue;
					}
					else
					{
						var isLinupRow = false;
						var m = linupTableRowRegexp.Match(line);
						if (m.Success && m.Groups.Count > 3)
						{
							var player = m.Groups[1].Value.Trim();
							var number = m.Groups[2].Value.Trim();
							var position = m.Groups[3].Value.Trim();
							if (lineupDict.ContainsKey(currentDict))
							{
								lineupDict[currentDict].Add(new Tuple<string, string, string>(player, number, position));

							}
							isLinupRow = true;
						}
						if (!isLinupRow)
						{
							m = benchTableRowRegexp.Match(line);
							if (m.Success && m.Groups.Count > 2)
							{
								var player = m.Groups[1].Value.Trim();
								var number = m.Groups[2].Value.Trim();
								benchList.Add(new Tuple<string, string>(player, number));
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
		if (benchList.Any())
		{
			lines.Add($"#Bench");
			foreach (var playerTuple in benchList)
			{
				lines.Add($"{playerTuple.Item1};{playerTuple.Item2}");
			}
		}
		if (noteLines.Any())
		{
			lines.Add($"***");
			foreach(var noteLine in noteLines)
			{
				lines.Add(noteLine);
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

if (!testMode)
{
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
}
