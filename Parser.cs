

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GameGenerator
{
	public class Parser
	{
		private static Regex titleRegexp = new Regex("<title>(.*)<\\/title>");

		const string noCacheMeta = "<meta http-equiv=\"Cache-Control\" content=\"no-cache, no-store, must-revalidate\"/><meta http-equiv=\"Pragma\" content=\"no-cache\"/><meta http-equiv=\"Expires\" content=\"0\"/>";
		const string lineupTemplate = "<!DOCTYPE html><html lang=\"en\"><head>#NO_CACHE_META#<meta charset=\"UTF-8\"/><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"/><link rel=\"stylesheet\" href=\"../../../style.css\"><title>#TITLE#</title></head><body><div class=\"exported-note\"><div class=\"exported-note-title\">#TITLE#</div><div id=\"rendered-md\">#TABLES#</div>#NOTES#</div><h1 id=\"links\"><strong>Links</strong></h1><p><a title=\"https://riverabaseball.com\" href=\"https://riverabaseball.com\">Home</a>&nbsp;&nbsp;&nbsp;<a title=\"Youtube Playlist\" href=\"https://youtube.com/playlist?list=PLdbXG0VpP0Mbb1QiRGcgjs-2TNjjz-7X0&si=Y4wGk6xrwDaf5vEy\">YouTube Playlist</a>&nbsp;&nbsp;&nbsp;<a href=\"./archive/index.html\">Archived Lineups</a></p></div></div></body></html>";
		const string picturesIndexTemplate = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\" /><link rel=\"icon\" type=\"image/x-icon\" href=\"/images/favicon.ico\"/><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" /><link rel=\"stylesheet\" href=\"../../../../../style.css\"/><title>Pictures</title></head><body><div class=\"exported-note\"><div><h1 id=\"Pictures\"><strong>Pictures</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Player</th></tr></thead><tbody><!--<tr>\r\n\t\t\t\t\t\t\t<td><a href=\"EthanRivera.jpg\">Ethan Rivera</a> </td>\r\n\t\t\t\t\t\t</tr>--></table></div><h1 id=\"links\"><strong>Links</strong></h1><p class=\"bottomLinks\"><a href=\"https://riverabaseball.com\">Home</a><a href=\"https://www.youtube.com/playlist?list=PLdbXG0VpP0Mb4p5j_fUam-AX1btECUJNR\">YouTube Playlist</a><a href=\"../../archive/index.html\">Archived Lineups</a></p></div></div></body></html>";


		public static void UpdateLatestArchiveEntryWithYoutubeLink(string youtubeLink, string teamWebsiteRoot)
		{
			Console.WriteLine("UpdateLatestArchiveEntryWithYoutubeLink");
			var archiveFolder = Path.Combine(teamWebsiteRoot ?? string.Empty, "archive");
			var archiveIndex = Path.Combine(archiveFolder, "index.html");
			var dom = XDocument.Load(archiveIndex);
			var linkUpdated = false;
			if (dom?.Root != null)
			{
				Console.WriteLine("UpdateLatestArchiveEntryWithYoutubeLink:ParsingDOM");
				var nsp = dom.Root.Name.Namespace;
				var trNsp = nsp + "tr";
				var aNsp = nsp + "a";
				var lastRowLinks = dom.Root.Descendants(trNsp).LastOrDefault()?.Descendants(aNsp)?.Reverse() ?? Enumerable.Empty<XElement>();
				foreach (var link in lastRowLinks)
				{
					Console.WriteLine("UpdateLatestArchiveEntryWithYoutubeLink:LinkLoop");
					if ("Video".Equals(link.Value))
					{
						if ("_blank".Equals(link.Attribute("href")?.Value))
						{
							link.SetAttributeValue("href", youtubeLink);
							linkUpdated = true;
							break;
						}
					}
				}
				if (linkUpdated)
				{
					Console.WriteLine("UpdateLatestArchiveEntryWithYoutubeLink:linkUpdated");
					File.WriteAllText(archiveIndex, dom.ToString());
				}
				else
				{
					Console.WriteLine("UpdateLatestArchiveEntryWithYoutubeLink:linkNotUpdated");
				}
			}

		}
		public static bool WriteLineupTable(string path, string outputHTMLPath)
		{
			Console.WriteLine("WriteLineupTable");
			var res = ParseLineupFile(path);
			if (!string.IsNullOrWhiteSpace(res.Item1) && !string.IsNullOrWhiteSpace(res.Item2))
			{
				if (File.Exists(outputHTMLPath))
				{
					var title = GetGameTitleFromHTML(outputHTMLPath);
					//make sure this isnt just an updated lineup
					if (!string.IsNullOrWhiteSpace(title) && !title.Equals(res.Item1))
					{
						ArchiveHtmlFile(outputHTMLPath);
					}
					Console.WriteLine($"Deleting existing current lineup at: {outputHTMLPath}");
					File.Delete(outputHTMLPath);
				}
				Console.WriteLine($"GetGameTitleFromHTML writing: {outputHTMLPath} with title {res.Item2}");
				File.WriteAllText(outputHTMLPath, res.Item2);
				return true;
			}
			return false;
		}

		private static string GetGameTitleFromHTML(string htmlPath)
		{
			Console.WriteLine("GetGameTitleFromHTML");
			var title = string.Empty;
			if (File.Exists(htmlPath))
			{
				var allText = File.ReadAllText(htmlPath);
				var m = titleRegexp.Match(allText);
				if (m.Success && m.Groups.Count > 1)
				{
					title = m.Groups[1].Value;
				}
			}
			return title;
		}

		public static void ArchiveHtmlFile(string htmlFile, string replaceHtmlFile = "")
		{
			Console.WriteLine("ArchiveHtmlFile");
			if (!File.Exists(htmlFile))
			{
				Console.WriteLine($"{htmlFile} does not exist");
			}
			var allText = File.ReadAllText(htmlFile);
			var title = string.Empty;
			var m = titleRegexp.Match(allText);
			if (m.Success && m.Groups.Count > 1)
			{
				title = m.Groups[1].Value;
				Console.WriteLine($"title: {title}");
				if ("rivera baseball".Equals(title, StringComparison.CurrentCultureIgnoreCase))
				{
					Console.WriteLine($"attempting to archive offline lineup... returning");
					return;
				}
			}
			else
			{
				Console.WriteLine($"retrieve title failed...");
			}
			allText = allText.Replace("./archive/index.html", "../index.html").Replace("../style.css", "../../../style.css").Replace(noCacheMeta, string.Empty);
			var archiveFolder = Path.Combine(Path.GetDirectoryName(htmlFile) ?? string.Empty, "archive");
			var archiveIndex = Path.Combine(archiveFolder, "index.html");
			Console.WriteLine($"Archive Index: {archiveIndex}");
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
					Console.WriteLine($"Creating Game Folder: {outputFolder}");
					Directory.CreateDirectory(outputFolder);
				}
				if (Directory.Exists(outputFolder))
				{
					Console.WriteLine($"Game Folder: {outputFolder}");
					var outputFile = Path.Combine(outputFolder, "index.html");
					File.WriteAllText(outputFile, allText);
					if (File.Exists(archiveIndex))
					{
						UpdateArchiveHtml(archiveIndex, title, "index.html", htmlFile);
					}
				}

			}
			if (File.Exists(replaceHtmlFile))
			{
				try
				{
					File.WriteAllText(htmlFile, File.ReadAllText(replaceHtmlFile));
				}
				catch
				{
					//
				}
			}

		}

		private static void UpdateArchiveHtml(string path, string archivedGameFolderName, string outputHtmlFileName, string sourceHtmlPath)
		{
			Console.WriteLine($"UpdateArchiveHtml");
			if (!File.Exists(path))
			{
				Console.WriteLine($"UpdateArchiveHtml path does not exist {path}");
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
				else if (visitor.Equals("beaumont", StringComparison.InvariantCultureIgnoreCase))
				{
					visitor = "<img src=\"logo.png\"/>";
				}
				var home = m.Groups[2].Value.Trim();
				if (home.Equals("cubs", StringComparison.InvariantCultureIgnoreCase))
				{
					home = "<img src=\"cubs.png\"/>";
				}
				else if (home.Equals("beaumont", StringComparison.InvariantCultureIgnoreCase))
				{
					visitor = "<img src=\"logo.png\"/>";
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
					if (!Directory.Exists(picturesDir))
					{
						Console.WriteLine($"Creating pictures directory inside: {picturesDir}");
						Directory.CreateDirectory(picturesDir);
					}
					if (Directory.Exists(picturesDir))
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
				catch
				{
					//
				}
			}
		}

		public static Tuple<string, string> ParseLineupFile(string path)
		{
			var gameTitle = string.Empty;
			var isIntoNotes = false;
			if (File.Exists(path))
			{
				var text = File.ReadAllLines(path);
				var tableHtml = string.Empty;
				var notesHtml = string.Empty;
				int currentOrder = 1;
				foreach (var line in text)
				{
					if (isIntoNotes)
					{
						var noteLine = line;
						if (line.IndexOf("`") >= 0)
						{
							var join = new List<string>();
							var split = line.Split('`').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
							for (var i = 0; i < split.Count; i++)
							{
								if (i == 0)
								{
									join.Add(split[i]);
								}
								else
								{
									join.Add("(<b>");
									join.Add(split[i]);
									join.Add("</b>)");
								}
							}
							noteLine = string.Join("",join);
						}
						notesHtml += $"{noteLine}\r\n";
						continue;
					}
					var trimmed = line.Trim();
					if (line == "***")
					{
						isIntoNotes = true;
						notesHtml = "<pre>";
					}
					if (line.StartsWith("!"))
					{
						gameTitle = line[1..].Trim();
					}
					else if (line.StartsWith("#") && !line.Contains("Bench"))
					{
						currentOrder = 1;
						if (!string.IsNullOrWhiteSpace(tableHtml))
						{
							tableHtml += "</tbody></table>";
						}
						tableHtml += $"<h1 id=\"{trimmed[1..]}\"><strong>{trimmed[1..]}</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Order</th><th>Name</th><th>Number</th><th>Position</th></tr></thead><tbody>";
					}
					else if (line.StartsWith("#") && line.Contains("Bench"))
					{
						if (!string.IsNullOrWhiteSpace(tableHtml))
						{
							tableHtml += "</tbody></table>";
						}
						tableHtml += $"<h1 id=\"Bench\"><strong>Bench</strong></h1><div class=\"joplin-table-wrapper\"><table><thead><tr><th>Name</th><th>Number</th></tr></thead><tbody>";
					}

					else
					{
						var split = trimmed.Split(';');
						if (split.Length == 2)
						{
							tableHtml += $"<tr><td>{split[0]}</td><td>{split[1]}</td></tr>";
						}
						if (split.Length > 2)
						{
							tableHtml += $"<tr><td>{currentOrder++}</td><td>{split[0]}</td><td>{split[1]}</td><td>{split[2]}</td></tr>";
						}
					}
				}
				tableHtml += "</tbody></table>";
				if (!string.IsNullOrWhiteSpace(notesHtml))
				{
					notesHtml += "</pre>";
					notesHtml = $"<h1><strong>Notes</strong></h1>{notesHtml}";
				}
				return new Tuple<string, string>(gameTitle, lineupTemplate.Replace("#NO_CACHE_META#", noCacheMeta).Replace("#TITLE#", gameTitle).Replace("#TABLES#", tableHtml).Replace("#NOTES#", notesHtml));
			}
			return new Tuple<string, string>(string.Empty, string.Empty);
		}
	}
}
