using System.Collections.Generic;

namespace FileHistory.Core
{
    public class FilesToKeepAndDelete
	{
		public FilesToKeepAndDelete(List<FileHistoryFile> keepList, List<FileHistoryFile> deleteList)
        {
			KeepList = keepList;
			DeleteList = deleteList;
        }

		public IList<FileHistoryFile> KeepList { get; init; }

		public IList<FileHistoryFile> DeleteList { get; init; }
	}
}
