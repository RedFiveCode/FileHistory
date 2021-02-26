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

			if (recurseSubFolders)
			{
				// get sub-folders, and get file groups in each sub-folder individually
				var folders = fileSystem.Directory.EnumerateDirectories(path);

				// for each folder
				// get groups of files in that folder
				var allGroups = new List<FileHistoryGroup>();

				foreach (var folder in folders)
                {
					var groups = GetFolderGroupDetailsInternal(folder, wildcardFilter, minimumFileSize);

					allGroups.AddRange(groups);
				}

				return allGroups;
			}
			else
            {
				// get file groups in current folder
				return GetFolderGroupDetailsInternal(path, wildcardFilter, minimumFileSize);

			}
		}

		/// <summary>
		/// Get file groups in the specified folder
		/// </summary>
		/// <param name="path"></param>
		/// <param name="wildcardFilter"></param>
		/// <param name="minimumFileSize"></param>
		/// <returns></returns>
		private List<FileHistoryGroup> GetFolderGroupDetailsInternal(string path, string wildcardFilter, long minimumFileSize)
		{
			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			var wildcard = (String.IsNullOrEmpty(wildcardFilter) ? "*.*" : wildcardFilter);

			var files = fileSystem.Directory.EnumerateFiles(path, wildcard, SearchOption.TopDirectoryOnly);

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
