using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace FileHistory.Core
{
    [DebuggerDisplay("Name={Name}, Ext={Ext}, Time={Time}, FullPath={FullPath}")]
    public class FileHistoryFile
    {
        private IFileInfo Info;

        public FileHistoryFile(string path, string name, string extension, string timestamp)
            : this(new FileSystem(), path, name, extension, timestamp)
        { }

        public FileHistoryFile(IFileSystem fileSystem, string path, string name, string extension, string timestamp)
        {
            FullPath = path;
            Name = name;
            Extension = extension;
            Time = timestamp;
            Info = fileSystem.FileInfo.FromFileName(path);
        }

        /// <summary>
        /// Pathname 
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Filename 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File extension; optional (includes dot) 
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// File timestamp
        /// </summary>
        public string Time { get; private set; }

        /// <summary>
        /// Original Filename; excludes timestamp.
        /// </summary>
        public string RawName
        {
            get { return Name + " (" + Time + ")" + Extension; }
        }

        /// <summary>
        /// Original Filename including extension; excludes timestamp.
        /// </summary>
        public string FileName
        {
            get { return Name + Extension; }
        }

        /// <summary>
        /// Gets the size, in bytes, of the current file
        /// </summary>
        public long Length
        {
            get { return Info.Length; }
        }

        /// <summary>
        /// Get the creation time of the file
        /// </summary>
        public DateTime CreationTime
        {
            get { return Info.CreationTime; }
        }
    }
}