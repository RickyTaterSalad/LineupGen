
namespace GameGenerator
{
	public class Parser
	{
		const string lineupTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/><link rel=\"stylesheet\" href=\"../../../style.css\"><title>#TITLE#</title></head><body><div class=\"exported-note\"><div class=\"exported-note-title\">#TITLE#</div><div id=\"rendered-md\">#TABLES#<h1 id=\"links\"><strong>Links</strong></h1><p><a title=\"https://riverabaseball.com\" href=\"https://riverabaseball.com\">Home</a>&nbsp;&nbsp;&nbsp;<a title=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\" href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a>&nbsp;&nbsp;&nbsp;<a href=\"./archive/index.html\">Archived Lineups</a></p></div></div></body></html>";

		public static bool WriteLineupTable(string path, string outputFolder) {

			if (Directory.Exists(outputFolder))
			{
				var res = Parser.ParseLineupFile(path);
				if (!string.IsNullOrWhiteSpace(res.Item1) && !string.IsNullOrWhiteSpace(res.Item2))
				{
					var outFile = $"{res.Item1}.html";
					var outHtml = Path.Combine(outputFolder, outFile);
					File.WriteAllText(outHtml, res.Item2);
					return true;
				}
			}
			return false;
		}

		public static Tuple<string, string> ParseLineupFile(string path)
		{
			var gameTitle = string.Empty;
			if (File.Exists(path))
			{
				var text = File.ReadAllLines(path);
				var tableHtml = string.Empty;
				int currentOrder = 1;
				foreach (var line in text)
				{
					var trimmed = line.Trim();
					if (line.StartsWith("!"))
					{
						gameTitle = line[1..].Trim();
					}
					else if (line.StartsWith("#"))
					{
						currentOrder = 1;
						if (!string.IsNullOrWhiteSpace(tableHtml))
						{
							tableHtml += "</tbody></table>";
						}
						tableHtml += $"<h1 id=\"{trimmed[1..]}\"><strong>{trimmed[1..]}</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Order</th><th>Name</th><th>Number</th><th>Position</th></tr></thead><tbody>";
					}
					else
					{
						var split = trimmed.Split(';');
						if (split.Length > 2)
						{
							tableHtml += $"<tr><td>{currentOrder++}</td><td>{split[0]}</td><td>{split[1]}</td><td>{split[2]}</td></tr>";
						}
					}
				}
				tableHtml += "</tbody></table>";
				return new Tuple<string,string>(gameTitle, lineupTemplate.Replace("#TITLE#", gameTitle).Replace("#TABLES#", tableHtml));
			}
			return new Tuple<string, string>(string.Empty, string.Empty);
		}
	}
}
