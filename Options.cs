using CommandLine;

namespace LineupGen
{
	internal class Options
	{
		[Option('m', "mode", Required = false, HelpText = "Mode")]
		public string Mode { get; set; }

		[Option('u', "videoUrl", Required = false, HelpText = "Video URL")]
		public string VideoUrl { get; set; }

		[Option('r', "websiteRoot", Required = false, HelpText = "Website Root")]
		public string WebsiteRoot { get; set; }

		[Option('a', "archiveHtml", Required = false, HelpText = "Archive HTMl Path")]
		public string ArchiveHtmlPath { get; set; }

		[Option('t', "teamRootDirectory", Required = false, HelpText = "Team Root Directory")]
		public string TeamRootDirectory { get; set; }

		[Option('d', "mdDirectory", Required = false, HelpText = "MD Directory")]
		public string MDDirectory { get; set; }

		public bool IsSilent
		{
			get
			{
				return 
					"publish".Equals(Mode, StringComparison.InvariantCultureIgnoreCase) ||
					"offline".Equals(Mode, StringComparison.InvariantCultureIgnoreCase) ||
					"youtube".Equals(Mode, StringComparison.InvariantCultureIgnoreCase) ||
					"archive".Equals(Mode, StringComparison.InvariantCultureIgnoreCase);
			}
		}
		public MODE GetMode()
		{
			//	cmd = isPublishMode ? "3": (isTakeLineupOfflineMode ? "4" : isSetYoutubeLinkMode ? "5": isArchiveMode ? "2" : "1");
			switch (Mode?.ToLowerInvariant() ?? string.Empty)
			{
				case ("publish"):
					return MODE.PUBLISH;
				case ("offline"):
					return MODE.OFFLINE;
				case ("youtube"):
					return MODE.YOUTUBE;
				case ("archive"):
					return MODE.ARCHIVE;
			}
			return MODE.NONE;
		}
		internal enum MODE
		{
			PUBLISH,
			OFFLINE,
			YOUTUBE,
			ARCHIVE,
			NONE,
			TEST
		}
	}
}
