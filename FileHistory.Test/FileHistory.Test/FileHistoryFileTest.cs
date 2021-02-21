using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FileHistory.Test
{
    [TestClass]
    public class FileHistoryFileTest
    {
        [TestMethod]
        public void RawName_Returns_Concatanted_Values()
        {
            var target = new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename (2016_07_07 18_56_08 UTC).ext", target.RawName);
        }

        [TestMethod]
        public void FullName_Returns_Concatanted_Values()
        {
            var target = new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename.ext", target.FullName);
        }

        [TestMethod]
        public void Name_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("filename", target.Name);
        }

        [TestMethod]
        public void Ext_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual(".ext", target.Ext);
        }

        [TestMethod]
        public void Time_Returns_Expected_Value()
        {
            var target = new FileHistoryFile(@"\\server\somepath", "filename", ".ext", "2016_07_07 18_56_08 UTC");

            Assert.AreEqual("2016_07_07 18_56_08 UTC", target.Time);
        }
    }
}
