using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace FileHistory.Core
{
    [DebuggerDisplay("Name={Name}, Extension={Extension}, Time={Time}, FullPath={FullPath}")]
    public class FileHistoryFile
    {
        public FileHistoryFile(IFileSystem fileSystem, string path, string name, string extension, string timestamp)
            : this(fileSystem, path, name, extension, timestamp, fileSystem.FileInfo.New(path).CreationTime)
        {
        }

        public FileHistoryFile(IFileSystem fileSystem, string path, string name, string extension, string timestamp, DateTime created)
        {
            FullPath = path;
            Name = name;
            Extension = extension;
            Time = timestamp;
            CreationTime = created;

            // Always set Length using file info
            var info = fileSystem.FileInfo.New(path);
            Length = info.Length;
        }

        /// <summary>
        /// Pathname 
        /// </summary>
        public string FullPath { get; init; }

        /// <summary>
        /// Filename 
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// File extension; optional (includes dot) 
        /// </summary>
        public string Extension { get; init; }

        /// <summary>
        /// File timestamp
        /// </summary>
        public string Time { get; init; }

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
        public long Length { get; init; }

        /// <summary>
        /// Get the creation time of the file
        /// </summary>
        public DateTime CreationTime { get; init; }
    }
}