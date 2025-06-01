using System;
using System.Collections.Generic;

namespace FileHistory.Core
{
    public class FilesToKeepAndDelete
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesToKeepAndDelete"/> class.
        /// </summary>
        /// <param name="keepList">The list of files to keep.</param>
        /// <param name="deleteList">The list of files to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown if either list is null.</exception>
        public FilesToKeepAndDelete(List<FileHistoryFile> keepList, List<FileHistoryFile> deleteList)
        {
			ArgumentNullException.ThrowIfNull(keepList, nameof(keepList));
            ArgumentNullException.ThrowIfNull(deleteList, nameof(deleteList));

            KeepList = new List<FileHistoryFile>(keepList);
			DeleteList = new List<FileHistoryFile>(deleteList);
        }

        /// <summary>
        /// Gets the files to keep.
        /// </summary>
        public IList<FileHistoryFile> KeepList { get; init; }

        /// <summary>
        /// Gets the files to delete.
        /// </summary>
		public IList<FileHistoryFile> DeleteList { get; init; }
	}
}
