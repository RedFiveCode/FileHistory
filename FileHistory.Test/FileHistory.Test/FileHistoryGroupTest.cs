using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

namespace FileHistory.Test
{
    /// <summary>
    /// Summary description for FileHistoryGroupTest
    /// </summary>
    [TestClass]
    public class FileHistoryGroupTest
    {

        [TestMethod]
        public void FileHistoryGroup_Ctor_Sets_Properties()
        {
            var fhList = CreateFileHistoryFileList();

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual("filename", target.Name);
            Assert.AreEqual(".ext", target.Ext);
            Assert.IsNotNull(target.Files);
            CollectionAssert.AreEqual(fhList, target.Files);
        }

        [TestMethod]
        public void FileCount_Returns_NumberOfFiles()
        {
            var fhList = CreateFileHistoryFileList();

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(fhList.Count, target.FileCount);
        }

        [TestMethod]
        public void FileSize_Returns_TotalFileSize()
        {
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAnfAddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", "filename", ".ext", "2021_01_01 10_10_00 UTC", new DateTime(2021, 1, 1, 10, 10, 0)),
                CreateFileHistoryFileAnfAddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", "filename", ".ext", "2021_01_01 10_11_00 UTC", new DateTime(2021, 1, 1, 10, 12, 0)),
                CreateFileHistoryFileAnfAddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", "filename", ".ext", "2021_01_01 10_12_00 UTC", new DateTime(2021, 1, 1, 10, 12, 0)),
            };

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(36L, target.FileSize); // 3 files each with 12 bytes ("Test data...")
        }

        private FileHistoryFile CreateFileHistoryFileAnfAddToFileSystem(MockFileSystem mockFileSystem, string path, string filename, string extension, string timestamp, DateTime created)
        {
            mockFileSystem.AddFile(path, new MockFileData("Test data...") { CreationTime = created });

            return new FileHistoryFile(mockFileSystem, path, filename, extension, timestamp);
        }

        private List<FileHistoryFile> CreateFileHistoryFileList()
        {
            var list = new List<FileHistoryFile>()
            {
                new FileHistoryFile(@"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", "filename", ".ext", "2021_01_01 10_10_00 UTC"),
                new FileHistoryFile(@"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", "filename", ".ext", "2021_01_01 10_11_00 UTC"),
                new FileHistoryFile(@"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", "filename", ".ext", "2021_01_01 10_12_00 UTC"),
            };

            return list;
        }

    }
}
