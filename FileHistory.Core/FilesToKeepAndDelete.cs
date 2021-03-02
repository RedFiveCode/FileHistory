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

		public IList<FileHistoryFile> KeepList { get; private set; }

		public IList<FileHistoryFile> DeleteList { get; private set; }
	}
}
