﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileHistory.Core
{

    [DebuggerDisplay("Name={Name}, Extension={Extension}, Folder={Folder}")]
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
        public string Extension { get; private set; }

        /// <summary>
        /// Filename including extension
        /// </summary>
        public string Fullname
        {
            get { return Name + Extension; }
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
            ArgumentNullException.ThrowIfNullOrEmpty(fullname, nameof(fullname));
            ArgumentNullException.ThrowIfNull(files, nameof(files));

            if (files.Any())
            {
                Folder = Path.GetDirectoryName(files.First().FullPath);
            }

            Name = Path.GetFileNameWithoutExtension(fullname);
            Extension = Path.GetExtension(fullname);
            Files = new List<FileHistoryFile>(files);
        }

        public FilesToKeepAndDelete GetFilesToKeepAndDelete(int recentGenerationsToKeep)
        {
            if (recentGenerationsToKeep < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(recentGenerationsToKeep));
            }

            // files are ordered by time, keep the newest (first/Skip(1)) and delete the others
            var files = Files.OrderByDescending(x => x.CreationTime);

            var keepList = files.Take(recentGenerationsToKeep).ToList();

            var deleteList = files.Skip(recentGenerationsToKeep).ToList();

            return new FilesToKeepAndDelete(keepList, deleteList);
        }

    }

}
