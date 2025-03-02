using GameGenerator;
using System.Text.RegularExpressions;


var cmd = string.Empty;
var retries = 10;
var count = 0;
const string defaultWebsiteRoot = "C:\\Github\\BaseballWebsite";

const string defaultIndexHtml = "C:\\Github\\BaseballWebsite\\2025\\Mustang\\Cubs\\index.html";

var directoryArg = string.Empty;
var linkArg = string.Empty;
var isSilentMode = false;
var isScriptMode = false;
var isPublishMode = false;
var isTakeLineupOfflineMode = false;
var isSetYoutubeLinkMode = false;
var isArchiveMode = false;
foreach (var arg in args){
	if(Directory.Exists(arg)){
		directoryArg = arg;
		isSilentMode = isScriptMode = true;
		break;
	}
	if(arg?.StartsWith("http") ?? false){
		Console.WriteLine($"URL Passed: {arg}");
		linkArg = arg;
	}
	if(!isSilentMode && "silent".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Silent Mode");
		isSilentMode = true;
	}
	if(!isScriptMode && "script".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Script Mode");
		isScriptMode = true;
	}
	if(!isPublishMode && "publish".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Publish Mode");
		isPublishMode = true;
	}
	if(!isTakeLineupOfflineMode && "offline".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Offline Mode");
		isTakeLineupOfflineMode = true;
	}
	if(!isSetYoutubeLinkMode && "youtube".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Youtube Link Mode");
		isSetYoutubeLinkMode = true;
	}
		if(!isArchiveMode && "archive".Equals(arg,StringComparison.InvariantCultureIgnoreCase)){
		Console.WriteLine("Archive Mode");
		isArchiveMode = true;
	}
	
}
isSilentMode = isSilentMode || isScriptMode || isPublishMode || isTakeLineupOfflineMode || isSetYoutubeLinkMode || isArchiveMode;


if(!isSilentMode){
	Console.WriteLine("1 - Current Lineup (JOPLIN TABLE)");
	Console.WriteLine("2 - Archive Current");
	Console.WriteLine("3 - Publish Lineup");
	Console.WriteLine("4 - Take Lineup Offline");
	while (cmd != "1" && cmd != "2" && count++ < retries)
	{
		cmd  = Console.ReadLine();
	}
}
else{
	//silent
	cmd = isPublishMode ? "3": (isTakeLineupOfflineMode ? "4" : isSetYoutubeLinkMode ? "5": isArchiveMode ? "2" : "1");
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
		if (string.IsNullOrWhiteSpace(joplinTextPath))
		{
			joplinTextPath = !string.IsNullOrWhiteSpace(directoryArg) ? directoryArg : Parser.defaultLineupProcessingFolder;
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
		Console.WriteLine($"Using Table File: " + joplinTextPath);
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
		var outputHTMLPath = string.Empty;
		if(!isSilentMode){
			Console.WriteLine($"Path To Existing Lineup HTML: (Default: {defaultIndexHtml})");
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
}

else if (cmd == "2")
{
	var userProvidedPath = string.Empty;
	while (!File.Exists(userProvidedPath) && count++ < retries)
	{
		if(!isSilentMode){
			Console.WriteLine($"Path To Existing Lineup HTML: (Default: {defaultIndexHtml})");
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
	if(!isSilentMode){
		Console.WriteLine("Complete... Press enter to exit.");
		Console.ReadLine();
	}
}

else if (cmd == "3" || cmd == "4")
{
	Console.WriteLine("Replacing root index.html....");
	var websiteRoot = string.Empty;
	while (!File.Exists(websiteRoot) && count++ < retries)
	{
		if(!isSilentMode){
			Console.WriteLine($"Path To Website Root: (Default: {defaultWebsiteRoot})");
			websiteRoot = Console.ReadLine();
		}
		if (string.IsNullOrWhiteSpace(websiteRoot))
		{
			websiteRoot = defaultWebsiteRoot;
		}
		if(isSilentMode){
			break;
		}
	}
	if (Directory.Exists(defaultWebsiteRoot))
	{
		var idxToCopy = cmd == "3" ? "index_lineup.html" : "index_no_lineup.html";
		var templateFile = Path.Combine(defaultWebsiteRoot,"templates",idxToCopy);
		Console.WriteLine($"Template: {templateFile}");
		if(File.Exists(templateFile)){
			Console.WriteLine($"Copying...");
			File.Copy(templateFile,Path.Combine(websiteRoot,"index.html"),true);
			Console.WriteLine($"Copied.");
		}
	}
	if(!isSilentMode){
		Console.WriteLine("Complete... Press enter to exit.");
		Console.ReadLine();
	}
}
else if (cmd == "5")
{
	//set youtube link on latest archived lineup
	if(string.IsNullOrWhiteSpace(linkArg)){
		return;
	}
	Parser.UpdateLatestArchiveEntryWithYoutubeLink(linkArg, Path.GetDirectoryName(defaultIndexHtml) ?? string.Empty);
}
