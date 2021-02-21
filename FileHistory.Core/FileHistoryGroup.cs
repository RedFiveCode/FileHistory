using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileHistory.Core
{
    public class FileHistoryGroup
    {
        /// <summary>
        /// Filename 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File extension; optional (includes dot) 
        /// </summary>
        public string Ext { get; private set; }

        /// <summary>
        /// Filename including extension
        /// </summary>
        public string Fullname
        {
            get { return Name + Ext; }
        }

        /// <summary>
        /// Historic versions of this file
        /// </summary>
        public List<FileHistoryFile> Files { get; private set; }


        public int FileCount
        {
            get { return Files.Count;  }
        }

        public long FileSize
        {
            get { return Files.Sum(i => i.Info.Length); }
        }

        public FileHistoryGroup(string fullname, IEnumerable<FileHistoryFile> files)
        {
            Name = Path.GetFileNameWithoutExtension(fullname);
            Ext = Path.GetExtension(fullname);
            Files = new List<FileHistoryFile>(files);
        }
    }

}
