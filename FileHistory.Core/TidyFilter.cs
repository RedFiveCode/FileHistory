using System;
using System.Collections.Generic;
using System.Linq;

namespace FileHistory.Core
{

    public class TidyFilter
    {
		public FilesToKeepAndDelete GetFilesToKeepAndDelete(FileHistoryGroup group, int recentGenerationsToKeep)
		{
			if (group == null)
			{
				throw new ArgumentNullException(nameof(group));
			}

			if (recentGenerationsToKeep < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(recentGenerationsToKeep));
			}

			var keepList = new List<FileHistoryFile>();
			var deleteList = new List<FileHistoryFile>();

			// ignore if only one file, cannot have duplicates
			if (group.Files.Count == 1)
			{
				keepList.Add(group.Files.First());
			}
			else
			{
				// files are ordered by time, keep the newest (first/Skip(1)) and delete the others
				var files = group.Files.OrderByDescending(x => x.CreationTime);

				keepList.AddRange(files.Take(recentGenerationsToKeep));

				deleteList.AddRange(files.Skip(recentGenerationsToKeep));
			}

			return new FilesToKeepAndDelete(keepList, deleteList);
		}
	}
}
