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
        private static readonly Regex FileHistoryRegex = new(
            @".*\\(?<name>.*?) \((?<ts>\d\d\d\d_\d\d_\d\d \d\d_\d\d_\d\d UTC)\)(?<ext>\..*)?",
            RegexOptions.Compiled);

        private static readonly Regex TimestampRegex = new(
            @"(?<year>\d\d\d\d)_(?<month>\d\d)_(?<day>\d\d) (?<hour>\d\d)_(?<minute>\d\d)_(?<second>\d\d)",
            RegexOptions.Compiled);

        private readonly IFileSystem fileSystem;

		public FileHistoryDiscovery() : this(new FileSystem())
		{ }

		public FileHistoryDiscovery(IFileSystem fileSystem)
		{
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
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
		/// <param name="recurseSubFolders">True to recurse sub-folders</param>
		/// <param name="wildcardFilter">Optional wild-card; defaults to *.*</param>
		/// <param name="minimumFileSize">Optional minimum file size; defaults to zero meaning all file sizes</param>
		/// <returns></returns>
		public List<FileHistoryGroup> GetFolderGroupDetails(string path, bool recurseSubFolders, string wildcardFilter, long minimumFileSize)
		{
            ArgumentNullException.ThrowIfNullOrEmpty(path, nameof(path));

			if (recurseSubFolders)
			{
				// get sub-folders, and get file groups in each sub-folder individually
				var folders = fileSystem.Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories).ToList();

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
            ArgumentException.ThrowIfNullOrEmpty(filename);

            var match = FileHistoryRegex.Match(filename);
            if (match.Success)
            {
                return new FileHistoryFile(
                    fileSystem,
                    filename,
                    match.Groups["name"].Value,
                    match.Groups["ext"].Success ? match.Groups["ext"].Value : string.Empty,
                    match.Groups["ts"].Value,
                    ParseTimestamp(match.Groups["ts"].Value));
            }
            return null;
        }

		public bool IsMatchingFile(string filename)
		{
			return !String.IsNullOrEmpty(filename) && FileHistoryRegex.IsMatch(filename);
		}

        public DateTime ParseTimestamp(string dt)
        {
            var match = TimestampRegex.Match(dt);
            if (match.Success)
            {
                return new DateTime(
                    int.Parse(match.Groups["year"].Value),
                    int.Parse(match.Groups["month"].Value),
                    int.Parse(match.Groups["day"].Value),
                    int.Parse(match.Groups["hour"].Value),
                    int.Parse(match.Groups["minute"].Value),
                    int.Parse(match.Groups["second"].Value),
                    DateTimeKind.Utc);
            }
            return DateTime.MinValue;
        }
	}
}
