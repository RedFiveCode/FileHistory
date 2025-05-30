using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;
using FileHistory.Core;
using FileHistory.Utils;
using System.IO.Abstractions;

namespace FileHistory.App
{
    public class TidyOperation
    {
		private IFileSystem fileSystem;

		public TidyOperation() : this(new FileSystem())
		{ }

		/// <summary>
		/// ctor for unit testing
		/// </summary>
		/// <param name="fileSystem"></param>
		public TidyOperation(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}


		public void TidyFolder(TidyCommandLineOptions options)
		{
			if (!fileSystem.Directory.Exists(options.Folder))
			{
				ColorConsole.WriteLine($"Folder: {options.Folder} does not exist", ConsoleColor.Red);

				return;
			}

			ColorConsole.WriteLine($"Processing folder: {options.Folder}...", ConsoleColor.Gray);

			var fh = new FileHistoryDiscovery(fileSystem);

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
				ShowList(keepList, "Keep", ConsoleColor.Green, options.Verbose);

				Console.WriteLine();

				ShowList(deleteList, "Delete", ConsoleColor.Magenta, options.Verbose);

			}
            else
			{
				// really delete the files
				foreach (var f in deleteList)
				{
					if (options.Verbose)
					{
						ColorConsole.WriteLine($"  Delete {f.FullPath}...", ConsoleColor.Magenta);
					}

					fileSystem.File.Delete(f.FullPath);
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

		private void ShowList(List<FileHistoryFile> list, string caption, ConsoleColor colour, bool verbose)
        {
			if (list.Any())
			{
				var deleteSize = ByteSize.FromBytes(list.Sum(f => f.Length));
				var deleteOldest = list.OrderBy(f => f.CreationTime).First();
				var deleteNewest = list.OrderBy(f => f.CreationTime).Last();
				ColorConsole.WriteLine($"{caption} {list.Count:n0} file(s); Size {deleteSize:0.000 GB}; Newest {deleteNewest.CreationTime:yyyy-MMM-dd HH:mm:ss}; Oldest {deleteOldest.CreationTime:yyyy-MMM-dd HH:mm:ss}", colour);

				if (verbose)
				{
					foreach (var f in list)
					{
						ColorConsole.WriteLine($"  {caption} {f.FullPath}", colour);
					}
				}
			}
			else
            {
				ColorConsole.WriteLine($"{caption} No files", colour);
			}
		}
	}
}
