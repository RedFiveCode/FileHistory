using System;
using System.IO;
using System.Linq;
using FileHistory.Core;
using FileHistory.Utils;

namespace FileHistory.App
{
    public class ShowOperation
    {
		public void ShowFolder(ShowCommandLineOptions options)
		{
			if (!Directory.Exists(options.Folder))
			{
				ColorConsole.WriteLine($"Folder: {options.Folder} does not exist", ConsoleColor.Red);

				return;
			}

			ColorConsole.WriteLine($"Processing folder: {options.Folder}...", ConsoleColor.Gray);

			var fh = new FileHistory.Core.FileHistory();

			var fileGroups = fh.GetFolderGroupDetails(options.Folder, options.RecurseSubFolders, options.WildcardFilter, options.MinimumSize);

			if (fileGroups.Count == 0)
			{
				ColorConsole.WriteLine($"No matching files in {options.Folder}", ConsoleColor.Red);
				return;
			}

			foreach (var g in fileGroups) //.OrderBy(x => x.RootName))
			{
				ColorConsole.WriteLine($"{g.Fullname} ({g.Files.Count} matches) :", ConsoleColor.Yellow);

				// files are ordered by time, most recent first
				foreach (var f in g.Files.OrderByDescending(x => x.Info.CreationTime))
				{
					// output similar to dir
					ColorConsole.Write($" {f.Info.CreationTime} {f.Info.Length,15:n0}", ConsoleColor.Cyan);
					ColorConsole.Write($" {f.RawName}", ConsoleColor.Yellow);
					ColorConsole.WriteLine($" {f.Info.FullName}", ConsoleColor.Green);
				}

				Console.WriteLine();
			}

			// stats
			if (options.Verbose)
			{
				var groupCount = fileGroups.Count();
				var fileCount = fileGroups.Sum(g => g.FileCount);
				var fileSize = fileGroups.Sum(g => g.FileSize);

				ColorConsole.Write($"Groups: {groupCount:n0}; Files {fileCount:n0}; Size {fileSize:n0} bytes", ConsoleColor.Cyan);
			}
		}
	}
}
