

using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GameGenerator
{
	public class Parser
	{
		public const string defaultLineupProcessingFolder = "C:\\JoplinExports";

		const string lineupTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"/><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/><link rel=\"stylesheet\" href=\"../../../style.css\"><title>#TITLE#</title></head><body><div class=\"exported-note\"><div class=\"exported-note-title\">#TITLE#</div><div id=\"rendered-md\">#TABLES#<h1 id=\"links\"><strong>Links</strong></h1><p><a title=\"https://riverabaseball.com\" href=\"https://riverabaseball.com\">Home</a>&nbsp;&nbsp;&nbsp;<a title=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\" href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a>&nbsp;&nbsp;&nbsp;<a href=\"./archive/index.html\">Archived Lineups</a></p></div></div></body></html>";
		const string picturesIndexTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\" /><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><link rel=\"stylesheet\" href=\"../../../../../style.css\"/><title>Pictures</title></head><body><div class=\"exported-note\"><div><h1 id=\"Pictures\"><strong>Pictures</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Player</th></tr></thead><tbody><!--<tr>\r\n\t\t\t\t\t\t\t<td><a href=\"EthanRivera.jpg\">Ethan Rivera</a> </td>\r\n\t\t\t\t\t\t</tr>--></table></div><h1 id=\"links\"><strong>Links</strong></h1><p class=\"bottomLinks\"><a href=\"https://riverabaseball.com\">Home</a><a href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a><a href=\"../../archive/index.html\">Archived Lineups</a></p></div></div></body></html>";
		public static bool WriteLineupTable(string path, string outputHTMLPath)
		{
			var res = Parser.ParseLineupFile(path);
			if (!string.IsNullOrWhiteSpace(res.Item1) && !string.IsNullOrWhiteSpace(res.Item2))
			{
				if (File.Exists(outputHTMLPath))
				{
					ArchiveHtmlFile(outputHTMLPath);
				}
				File.WriteAllText(outputHTMLPath, res.Item2);
				return true;
			}
			return false;
		}
		public static void ArchiveHtmlFile(string htmlFile)
		{
			if (File.Exists(htmlFile))
			{
				var allText = File.ReadAllText(htmlFile);
				var titleRegexp = new Regex("<title>(.*)<\\/title>");
				var title = string.Empty;
				var m = titleRegexp.Match(allText);
				if (m.Success && m.Groups.Count > 1)
				{
					title = m.Groups[1].Value;
				}
				allText = allText.Replace("/archive/", "/").Replace("../style.css", "../../style.css");
				var archiveFolder = Path.Combine(Path.GetDirectoryName(htmlFile) ?? string.Empty, "archive");
				var archiveIndex = Path.Combine(archiveFolder, "index.html");
				if (!Directory.Exists(archiveFolder))
				{
					Directory.CreateDirectory(archiveFolder);
				}
				if (Directory.Exists(archiveFolder))
				{
					//create game folder, if it exists this has alread been archived
					var outputFolder = Path.Combine(archiveFolder, title);
					if (!Directory.Exists(outputFolder))
					{
						Directory.CreateDirectory(outputFolder);
						if (Directory.Exists(outputFolder))
						{
							var outputFile = Path.Combine(outputFolder, "index.html");
							File.WriteAllText(outputFile, allText);
							if (File.Exists(archiveIndex))
							{
								UpdateArchiveHtml(archiveIndex, title, "index.html", htmlFile);
							}
						}
					}
				}
			}
		}

		private static void UpdateArchiveHtml(string path, string archivedGameFolderName, string outputHtmlFileName, string sourceHtmlPath)
		{
			if (!File.Exists(path))
			{
				return;
			}
			var rootDir = Path.GetDirectoryName(sourceHtmlPath) ?? string.Empty;
			var picturesDir = Path.Combine(rootDir, "pictures");
			var archivedPicturesDirToCreate = Path.Combine(picturesDir, archivedGameFolderName);
			var outputRow = $"<tr><td><a href=\"./{archivedGameFolderName}/{outputHtmlFileName}\">Lineup</a></td><td>#DATE#</td><td>#VISITOR#</td><td>#HOME#</td><td>0-0</td><td><a href=\"_blank\">Video</a></td><td><a href=\"../pictures/{archivedGameFolderName}/index.html\">Pictures</a></td><td><ul class=\"archiveNoteList\"></ul></td></tr>";
			var detailsRegexp = new Regex("^Game\\s{1}[0-9]+\\s{1}(.*)\\s{1}Vs\\.\\s{1}(.*)\\((.*)\\).*");
			var m = detailsRegexp.Match(archivedGameFolderName);
			if (m.Success && m.Groups.Count > 3)
			{
				var visitor = m.Groups[1].Value.Trim();
				if (visitor.Equals("cubs", StringComparison.InvariantCultureIgnoreCase))
				{
					visitor = "<img src=\"cubs.png\"/>";
				}
				var home = m.Groups[2].Value.Trim();
				if (home.Equals("cubs", StringComparison.InvariantCultureIgnoreCase))
				{
					home = "<img src=\"cubs.png\"/>";
				}
				var dateString = m.Groups[3].Value.Trim();
				outputRow = outputRow.Replace("#VISITOR#", visitor).Replace("#HOME#", home).Replace("#DATE#", dateString);
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
				return new Tuple<string, string>(gameTitle, lineupTemplate.Replace("#TITLE#", gameTitle).Replace("#TABLES#", tableHtml));
			}
			return new Tuple<string, string>(string.Empty, string.Empty);
		}
	}
}
