﻿using System;
using System.Diagnostics;
using System.IO;

namespace FileHistory.Core
{
    [DebuggerDisplay("Name={Name}, Ext={Ext}, Time={Time}")]
    public class FileHistoryFile
    {
        public FileHistoryFile(string path, string name, string extension, string timestamp)
        {
            FullPath = path;
            Name = name;
            Ext = extension;
            Time = timestamp;
            Info = new FileInfo(path);
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
        public string Ext { get; private set; }

        /// <summary>
        /// File timestamp
        /// </summary>
        public string Time { get; private set; }

        /// <summary>
        /// File info
        /// </summary>
        public FileInfo Info { get; private set; }

        /// <summary>
        /// Original Filename; excludes timestamp.
        /// </summary>
        public string RawName
        {
            get { return Name + " (" + Time + ")" + Ext; }
        }

        /// <summary>
        /// Original Filename including extension; excludes timestamp.
        /// </summary>
        public string FileName
        {
            get { return Name + Ext; }
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