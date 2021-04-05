using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;
using FileHistory.Core;
using FileHistory.Utils;

namespace FileHistory.App
{
    public class TidyOperation
    {
		public void TidyFolder(TidyCommandLineOptions options)
		{
			if (!Directory.Exists(options.Folder))
			{
				ColorConsole.WriteLine($"Folder: {options.Folder} does not exist", ConsoleColor.Red);

				return;
			}

			ColorConsole.WriteLine($"Processing folder: {options.Folder}...", ConsoleColor.Gray);

			var fh = new FileHistoryDiscovery();

			var fileGroups = fh.GetFolderGroupDetails(options.Folder, options.RecurseSubFolders, options.WildcardFilter, options.MinimumSize);

			if (fileGroups.Count == 0)
			{
				ColorConsole.WriteLine($"No matching files in {options.Folder}", ConsoleColor.Red);
				return;
			}

			var filesToKeepAndDelete = GetFilesToKeepAndDelete(fileGroups, options.KeepGenerations);

			var keepList = filesToKeepAndDelete.Item1;
			var deleteList = filesToKeepAndDelete.Item2;

			if (options.Preview)
            {
				var keepSize = ByteSize.FromBytes(keepList.Sum(f => f.Length));
				var keepOldest = keepList.OrderBy(f => f.CreationTime).First();
				var keepNewest = keepList.OrderBy(f => f.CreationTime).Last();
				ColorConsole.WriteLine($"Keep {keepList.Count:n0} file(s); Size {keepSize:0.000 MB}; Newest {keepNewest.CreationTime:yyyy-MMM-dd HH:mm:ss}; Oldest {keepOldest.CreationTime:yyyy-MMM-dd HH:mm:ss}", ConsoleColor.Green);

				if (options.Verbose)
				{
					foreach (var f in keepList)
					{
						ColorConsole.WriteLine($"  Keep   {f.FullPath}", ConsoleColor.Green);
					}
				}

				Console.WriteLine();
				var deleteSize = ByteSize.FromBytes(deleteList.Sum(f => f.Length));
				var deleteOldest = deleteList.OrderBy(f => f.CreationTime).First();
				var deleteNewest = deleteList.OrderBy(f => f.CreationTime).Last();
				ColorConsole.WriteLine($"Delete {deleteList.Count:n0} file(s); Size {deleteSize:0.000 MB}; Newest {deleteNewest.CreationTime:yyyy-MMM-dd HH:mm:ss}; Oldest {deleteOldest.CreationTime:yyyy-MMM-dd HH:mm:ss}", ConsoleColor.Magenta);

				if (options.Verbose)
				{
					foreach (var f in deleteList)
					{
						ColorConsole.WriteLine($"  Delete {f.FullPath}", ConsoleColor.Magenta);
					}
				}
			}
            else
			{
				foreach (var f in deleteList)
				{
					if (options.Verbose)
					{
						ColorConsole.WriteLine($"  Delete {f.FullPath}...", ConsoleColor.Magenta);
					}

					//File.Delete(f.Info.FullName);
				}
			}
		}

		private Tuple<List<FileHistoryFile>, List<FileHistoryFile>> GetFilesToKeepAndDelete(List<FileHistoryGroup> fileGroups, int recentGenerationsToKeep)
		{
			var keepList = new List<FileHistoryFile>();
			var deleteList = new List<FileHistoryFile>();

			foreach (var g in fileGroups)
			{
				// ignore if only one file, cannot have duplicates
				if (g.Files.Count == 1)
				{
					keepList.Add(g.Files.First());
				}
				else
				{
					// files are ordered by time, keep the newest (first/Skip(1)) and delete the others
					var files = g.Files.OrderByDescending(x => x.CreationTime);

					keepList.AddRange(files.Take(recentGenerationsToKeep));

					deleteList.AddRange(files.Skip(recentGenerationsToKeep));
				}
			}

			return new Tuple<List<FileHistoryFile>, List<FileHistoryFile>>(keepList, deleteList);
		}
	}
}
