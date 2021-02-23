using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileHistory.Core
{

    [DebuggerDisplay("Name={Name}, Ext={Ext}, Folder={Folder}")]
    public class FileHistoryGroup
    {
        /// <summary>
        /// Folder name 
        /// </summary>
        public string Folder { get; private set; }

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
            get { return Files.Sum(i => i.Length); }
        }

        public FileHistoryGroup(string fullname, IEnumerable<FileHistoryFile> files)
        {
            Folder = Path.GetDirectoryName(fullname);
            Name = Path.GetFileNameWithoutExtension(fullname);
            Ext = Path.GetExtension(fullname);
            Files = new List<FileHistoryFile>(files);
        }
    }

}
