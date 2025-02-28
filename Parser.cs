
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GameGenerator
{
	public class Parser
	{
		const string lineupTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"/><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/><link rel=\"stylesheet\" href=\"../../../style.css\"><title>#TITLE#</title></head><body><div class=\"exported-note\"><div class=\"exported-note-title\">#TITLE#</div><div id=\"rendered-md\">#TABLES#<h1 id=\"links\"><strong>Links</strong></h1><p><a title=\"https://riverabaseball.com\" href=\"https://riverabaseball.com\">Home</a>&nbsp;&nbsp;&nbsp;<a title=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\" href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a>&nbsp;&nbsp;&nbsp;<a href=\"./archive/index.html\">Archived Lineups</a></p></div></div></body></html>";
		const string picturesIndexTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\" /><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><link rel=\"stylesheet\" href=\"../../../../../style.css\"/><title>Pictures</title></head><body><div class=\"exported-note\"><div class=\"exported-note-title\">Pictures</div><div><h1 id=\"Pictures\"><strong>Pictures</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Player</th></tr></thead><tbody><!--<tr>\r\n\t\t\t\t\t\t\t<td><a href=\"EthanRivera.jpg\">Ethan Rivera</a> </td>\r\n\t\t\t\t\t\t</tr>--></table></div><h1 id=\"links\"><strong>Links</strong></h1><p class=\"bottomLinks\"><a href=\"https://riverabaseball.com\">Home</a><a href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a><a href=\"../../archive/index.html\">Archived Lineups</a></p></div></div></body></html>";
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
		public static void UpdateArchiveHtml(string path, string archivedHtml, string sourceHtmlPath){
			if(!File.Exists(path) || !File.Exists(archivedHtml)){
				return;
			}
			var rootDir = Path.GetDirectoryName(sourceHtmlPath) ?? string.Empty;
			var picturesDir = Path.Combine(rootDir, "pictures");
			var archiveFileNameNoExtension = Path.GetFileNameWithoutExtension(archivedHtml);
			var archivedPicturesDirToCreate = Path.Combine(picturesDir, archiveFileNameNoExtension);
			var outputRow = $"<tr><td><a href=\"#FILENAME#\">Lineup</a></td><td>#DATE#</td><td>#VISITOR#</td><td>#HOME#</td><td>0-0</td><td><a href=\"_blank\">Video</a></td><td><a href=\"../pictures/{archiveFileNameNoExtension}/index.html\">Pictures</a></td></tr>";
			var fileName = Path.GetFileName(archivedHtml);
			var detailsRegexp = new Regex("^Game\\s{1}[0-9]+\\s{1}(.*)\\s{1}Vs\\.\\s{1}(.*)\\((.*)\\).*");
			var m = detailsRegexp.Match(archiveFileNameNoExtension);
			if (m.Success && m.Groups.Count > 3)
			{
				var visitor = m.Groups[1].Value.Trim();
				if(visitor.Equals("cubs",StringComparison.InvariantCultureIgnoreCase)){
					visitor = "<img src=\"cubs.png\"/>";
				}
				var home = m.Groups[2].Value.Trim();
				if(home.Equals("cubs",StringComparison.InvariantCultureIgnoreCase)){
					home = "<img src=\"cubs.png\"/>";
				}
				var dateString = m.Groups[3].Value.Trim();
				outputRow = outputRow.Replace("#FILENAME#",fileName).Replace("#VISITOR#",visitor).Replace("#HOME#",home).Replace("#DATE#",dateString);
			}
			var dom = XDocument.Load(path);
			if (dom?.Root != null)
			{
				var nsp = dom.Root.Name.Namespace;
				var tbodyNsp = nsp + "tbody";
				var tBody = dom.Root.Descendants(tbodyNsp).FirstOrDefault();
				if (tBody != null)
				{
					var newRow = XElement.Parse(outputRow);
					tBody.Add(newRow);
				}
				File.WriteAllText(path, dom.ToString());

				try
				{
					//create pictures folder and html
					if (Directory.Exists(picturesDir))
					{
						if (!Directory.Exists(archivedPicturesDirToCreate))
						{
							if (!Directory.Exists(archivedPicturesDirToCreate))
							{
								Directory.CreateDirectory(archivedPicturesDirToCreate);
							}
							if (Directory.Exists(archivedPicturesDirToCreate))
							{
								File.WriteAllText(Path.Combine(archivedPicturesDirToCreate, "index.html"), picturesIndexTemplate);
							}
						}
					}
				}
				catch
				{
					//
				}
			}
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
