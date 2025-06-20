﻿using System;
using System.IO;
using System.Linq;
using ByteSizeLib;
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

			var fh = new FileHistoryDiscovery();

			var fileGroups = fh.GetFolderGroupDetails(options.Folder, options.RecurseSubFolders, options.WildcardFilter, options.MinimumSize);

			if (options.Hide) // hide files with a single generation
			{
				fileGroups = fileGroups.Where(g => g.FileCount >= 2).ToList();
			}

			if (fileGroups.Count == 0)
			{
				ColorConsole.WriteLine($"No matching files in {options.Folder}", ConsoleColor.Red);
				return;
			}

			foreach (var g in fileGroups) //.OrderBy(x => x.RootName))
			{
				var groupSize = ByteSize.FromBytes(g.Files.Sum(f => f.Length));

                ColorConsole.WriteLine($"{g.Fullname} ({g.Files.Count} matches; Size {groupSize:0.000 GB})", ConsoleColor.Yellow);

				// files are ordered by time, most recent first
				foreach (var f in g.Files.OrderByDescending(x => x.CreationTime))
				{
					// output similar to dir
					ColorConsole.Write($" {f.CreationTime} {f.Length,15:n0}", ConsoleColor.Cyan);
					ColorConsole.Write($" {f.RawName}", ConsoleColor.Yellow);
					ColorConsole.WriteLine($" {f.FullPath}", ConsoleColor.Green);
				}

				Console.WriteLine();
			}

			// stats
			if (options.Verbose)
			{
				var groupCount = fileGroups.Count();
				var fileCount = fileGroups.Sum(g => g.FileCount);
				var fileSize = fileGroups.Sum(g => g.FileSize);
				var size = ByteSize.FromBytes(fileSize);

				ColorConsole.WriteLine($"Groups: {groupCount:n0}", ConsoleColor.Cyan);
				ColorConsole.WriteLine($"Files:  {fileCount:n0}", ConsoleColor.Cyan);
				ColorConsole.WriteLine($"Size:   {size:0.000 GB}", ConsoleColor.Cyan);
			}
		}
	}
}
