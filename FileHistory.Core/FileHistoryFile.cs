using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace FileHistory.Core
{
    [DebuggerDisplay("Name={Name}, Extension={Extension}, Time={Time}, FullPath={FullPath}")]
    public class FileHistoryFile
    {

        public FileHistoryFile(IFileSystem fileSystem, string path, string name, string extension, string timestamp)
        {
            FullPath = path;
            Name = name;
            Extension = extension;
            Time = timestamp;
            var info = fileSystem.FileInfo.FromFileName(path);

            CreationTime = info.CreationTime;
        }

        public FileHistoryFile(IFileSystem fileSystem, string path, string name, string extension, string timestamp, DateTime created)
        {
            FullPath = path;
            Name = name;
            Extension = extension;
            Time = timestamp;
            CreationTime = created;

            // the file creation timestamp in the filename may differ from the timestamp from the file info object
            // so only use the file info object to get the file size
            var info = fileSystem.FileInfo.FromFileName(path);
            Length = info.Length;
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
        /// Original Filename; includes timestamp.
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
        public long Length { get; private set; }

        /// <summary>
        /// Get the creation time of the file
        /// </summary>
        public DateTime CreationTime { get; private set; }
    }
}