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

			// files are ordered by time, keep the newest (first/Skip(1)) and delete the others
			var files = group.Files.OrderByDescending(x => x.CreationTime);

			var keepList = files.Take(recentGenerationsToKeep).ToList();

			var deleteList = files.Skip(recentGenerationsToKeep).ToList();

			return new FilesToKeepAndDelete(keepList, deleteList);
		}
	}
}
