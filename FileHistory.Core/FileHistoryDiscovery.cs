using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileHistory.Core
{
	public class FileHistoryDiscovery
	{
		private IFileSystem fileSystem;

		public FileHistoryDiscovery() : this(new FileSystem())
		{ }

		public FileHistoryDiscovery(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}
		public List<FileHistoryGroup> GetFolderGroupDetails(string path)
		{ 
			return GetFolderGroupDetails(path, false, String.Empty, 0L);
		}

		public List<FileHistoryGroup> GetFolderGroupDetails(string path, bool recurseSubFolders, string wildcardFilter, long minimumFileSize)
		{
			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			var wildcard = (String.IsNullOrEmpty(wildcardFilter) ? "*.*" : wildcardFilter);
			var options = (recurseSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

			var files = fileSystem.Directory.EnumerateFiles(path, wildcard, options);

			// get files in current folders
			// and if recusrive, get sub-folders, and process each folder individually
			// want to avoid any interference between groups of files in different folders that have the same name
			if (recurseSubFolders)
			{
				fileSystem.Directory.EnumerateDirectories(path, "*.*", SearchOption.AllDirectories);

				// for each folder
				// get groups of files in that folder
			}

			// perhaps better to move the folder recursion logic into a calling function???


			var details = files.Select(f => GetFileDetails(f))
							   .ToList();

			details.RemoveAll(f => f == null); // remove null entries (for files that are not file history records)

			if (minimumFileSize > 0) // optionally remove files smaller than minimum file size
			{
				details.RemoveAll(f => f.Length < minimumFileSize);
			}

			var grouping = details.GroupBy(fd => fd.FileName)
								  .Select(fd => new { RootName = fd.Key, Items = fd.ToList() })
								  .OrderBy(g => g.RootName)
								  .Select(g => new FileHistoryGroup(g.RootName, g.Items))
								  .ToList()
								;

			return grouping;
		}

		public FileHistoryFile GetFileDetails(string path)
		{
			if (String.IsNullOrEmpty(path))
            {
				throw new ArgumentNullException(nameof(path));
            }

			// does not match files with no extension
			//var regex = new Regex(@".*\\(.*?) \((\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(\..*)?");
			var regex = new Regex(@".*\\(?<name>.*?) \((?<ts>\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(?<ext>\..*)?");

			if (regex.IsMatch(path))
			{
				var match = regex.Match(path);

				FileHistoryFile details = null;

				if (match.Groups.Count == 3)
                {
					details = new FileHistoryFile(fileSystem,
												    path,
													match.Groups["name"].Value,
													String.Empty,
													match.Groups["ts"].Value);
                }
				else if (match.Groups.Count == 4) // optional extension
				{
					details = new FileHistoryFile(fileSystem,
												  path,
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
