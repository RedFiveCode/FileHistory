using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace FileHistory.Test
{
    [TestClass]
    public class FileHistoryFileTest
    {
        [TestMethod]
        public void RawName_Returns_Concatanted_Values()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename (2016_07_07 18_56_08 UTC).ext", target.RawName);
        }

        [TestMethod]
        public void FileName_Returns_Concatenated_Values()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename.ext", target.FileName);
        }

        [TestMethod]
        public void FullPath_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", target.FullPath);
        }

        [TestMethod]
        public void Name_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename", target.Name);
        }

        [TestMethod]
        public void Ext_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual(".ext", target.Extension);
        }

        [TestMethod]
        public void Time_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("2016_07_07 18_56_08 UTC", target.Time);
        }

        [TestMethod]
        public void CreationTime_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext",
                                   new MockFileData("Test data...") { CreationTime = new DateTime(2016, 7, 7, 18, 56, 8) });

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            var expected = new DateTime(2016, 7, 7, 18, 56, 8);
            Assert.AreEqual(expected, target.CreationTime);
        }
    }
}
