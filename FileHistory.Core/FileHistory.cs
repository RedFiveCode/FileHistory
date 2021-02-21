using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileHistory.Core
{
    public class FileHistory
    {
		public void ShowFolderOld(string folder)
		{
			var files = GetFolderDetails(folder);

			if (files.Count == 0)
			{
				Console.WriteLine($"No matching files in {folder}");
				return;
			}

			var grouping = files.GroupBy(fd => fd.Name)
					.Select(fd => new { RootName = fd.Key, Items = fd.ToList() })
					;

			foreach (var g in grouping.OrderBy(x => x.RootName))
			{
				Console.WriteLine($"{g.RootName} ({g.Items.Count()} matches) :");

				// ignore if only one file, cannot have duplicates
				//if (g.Items.Count == 1)
				//{
				//	var m = g.Items.First();
				//	Console.WriteLine($" {m.Info.CreationTime} {m.Info.Length,8} {m.Info.FullName}");

				//	continue;
				//}

				// only interested in big files
				//if (g.Items.First().Info.Length < (1 * 1000 * 1000))
				//{
				//	continue;
				//}

				//Console.WriteLine($"{g.RootName} ({g.Items.Count()} matches) :");

				// TODO check duplicate names for (same name, same size)

				// files are ordered by time, most recent first
				foreach (var m in g.Items.OrderByDescending(x => x.Info.CreationTime))
				{
					// output similar to dir
					Console.WriteLine($" {m.Info.CreationTime} {m.Info.Length,8} {m.Info.FullName}");
				}

				Console.WriteLine();
			}
		}

		public List<FileHistoryGroup> GetFolderGroupDetails(string path, bool recurseSubFolders, string wildcardFilter, long minimumFileSize)
		{
			var wildcard = (String.IsNullOrEmpty(wildcardFilter) ? "*.*" : wildcardFilter);
			var options = (recurseSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			var files = Directory.EnumerateFiles(path, wildcard, options);

			// get files in current folders
			// and if recusrive, get sub-folders, and process each folder individually
			// want to avoid any interference between groups of files in different folders that have the same name
			if (recurseSubFolders)
			{
				Directory.EnumerateDirectories(path, "*.*", SearchOption.AllDirectories);

				// for each folder
				// get groups of files in that folder
			}

			// perhaps better to move the folder recursion logic into a calling function???


			var details = files.Select(f => GetFileDetails(f))
							   .ToList();

			details.RemoveAll(f => f == null); // remove null entries (for files that are not file history records)

			if (minimumFileSize > 0) // optionally remove files smaller than minimum file size
			{
				details.RemoveAll(f => f.Info.Length < minimumFileSize);
			}

			var grouping = details.GroupBy(fd => fd.FullName)
								  .Select(fd => new { RootName = fd.Key, Items = fd.ToList() })
								  .OrderBy(g => g.RootName)
								  .Select(g => new FileHistoryGroup(g.RootName, g.Items))
								  .ToList()
								;

			return grouping;
		}

		public List<FileHistoryFile> GetFolderDetails(string path)
		{
			var files = Directory.EnumerateFiles(path);
			var details = files.Select(f => GetFileDetails(f))
								.ToList();
			details.RemoveAll(f => f == null); // remove null entries (for files that are not file history records)

			return details;
		}

		public FileHistoryFile GetFileDetails(string path)
		{
			var file = Path.GetFileName(path);
			var details = Split(path);

			return details;
		}

		public FileHistoryFile Split(string line)
		{
			// does not match files with no extension
			//var regex = new Regex(@".*\\(.*?) \((\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(\..*)?");
			var regex = new Regex(@".*\\(?<name>.*?) \((?<ts>\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(?<ext>\..*)?");

			if (regex.IsMatch(line))
			{
				var match = regex.Match(line);

				FileHistoryFile details = null;

				if (match.Groups.Count == 3)
                {
					details = new FileHistoryFile(line, match.Groups["name"].Value,
													String.Empty,
													match.Groups["ts"].Value);
                }
				else if (match.Groups.Count == 4) // optional extension
				{
					details = new FileHistoryFile(line,
												  match.Groups["name"].Value,
												  match.Groups["ext"].Value,
												  match.Groups["ts"].Value);
				}

				return details;
			}

			// no match for files that are not file history records
			return null;
		}
	}
}
