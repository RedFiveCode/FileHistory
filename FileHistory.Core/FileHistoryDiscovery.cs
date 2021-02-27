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

		/// <summary>
		/// Get file history group objects for the specified folder
		/// </summary>
		/// <param name="path">Folder path</param>
		/// <returns></returns>
		public List<FileHistoryGroup> GetFolderGroupDetails(string path)
		{ 
			return GetFolderGroupDetails(path, false, String.Empty, 0L);
		}

		/// <summary>
		/// Get file history group objects for the specified folder with options.
		/// </summary>
		/// <param name="path">Folder path</param>
		/// <param name="recurseSubFolders">True to recuse sub-folders</param>
		/// <param name="wildcardFilter">Optional wildcars; defaults to *.*</param>
		/// <param name="minimumFileSize">Optional minimum file size; defaults to zero meaning all file sizes</param>
		/// <returns></returns>
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
		/// Get file history groups in the specified folder
		/// </summary>
		/// <param name="path"></param>
		/// <param name="wildcardFilter"></param>
		/// <param name="minimumFileSize"></param>
		/// <returns></returns>
		private List<FileHistoryGroup> GetFolderGroupDetailsInternal(string path, string wildcardFilter, long minimumFileSize)
		{
			var wildcard = (String.IsNullOrEmpty(wildcardFilter) ? "*.*" : wildcardFilter);

			var files = fileSystem.Directory.EnumerateFiles(path, wildcard, SearchOption.TopDirectoryOnly);

			var details = files.Where(f => IsMatchingFile(f)) // ignore files that are not file history records
							   .Select(f => GetFileDetails(f))
							   .ToList();

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

		/// <summary>
		/// Get the file history for the specified filename
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public FileHistoryFile GetFileDetails(string filename)
		{
			if (String.IsNullOrEmpty(filename))
            {
				throw new ArgumentNullException(nameof(filename));
            }

			// does not match files with no extension
			var regex = new Regex(@".*\\(?<name>.*?) \((?<ts>\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(?<ext>\..*)?");

			if (regex.IsMatch(filename))
			{
				var match = regex.Match(filename);

				FileHistoryFile details = null;

				if (match.Groups.Count == 4) 
				{
					details = new FileHistoryFile(fileSystem,
												  filename,
												  match.Groups["name"].Value,
												  match.Groups["ext"].Value, // will be empty if file has no extension
												  match.Groups["ts"].Value);
				}

				return details;
			}

			// no match for files that are not file history records
			return null;
		}

		public bool IsMatchingFile(string filename)
        {
			if (String.IsNullOrEmpty(filename))
			{
				return false;
			}

			var regex = new Regex(@".*\\(?<name>.*?) \((?<ts>\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(?<ext>\..*)?");

			return regex.IsMatch(filename);
		}
	}
}
