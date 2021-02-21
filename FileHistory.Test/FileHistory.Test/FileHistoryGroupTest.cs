using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var fhList = CreateFileHistoryFileList();

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(0L, target.FileSize);
        }

        private List<FileHistoryFile> CreateFileHistoryFileList()
        {
            var list = new List<FileHistoryFile>()
            {
                new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2021_01_01 10_10_00 UTC"),
                new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2021_01_01 10_11_00 UTC"),
                new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2021_01_01 10_12_00 UTC"),
            };

            return list;
        }
    }
}
