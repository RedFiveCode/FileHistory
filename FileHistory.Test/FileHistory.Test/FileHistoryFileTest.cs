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
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual("filename (2016_07_07 18_56_08 UTC).ext", target.RawName);
        }

        [TestMethod]
        public void FileName_Returns_Concatenated_Values()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual("filename.ext", target.FileName);
        }

        [TestMethod]
        public void FullPath_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", target.FullPath);
        }

        [TestMethod]
        public void Name_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual("filename", target.Name);
        }

        [TestMethod]
        public void Ext_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual(".ext", target.Extension);
        }

        [TestMethod]
        public void Time_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            Assert.AreEqual("2016_07_07 18_56_08 UTC", target.Time);
        }

        [TestMethod]
        public void CreationTime_Returns_Expected_Value()
        {
            var mockFileSystem = new MockFileSystem();
            var created = new DateTime(2016, 7, 7, 18, 56, 8);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", created);

            var target = new FileHistoryFile(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", "filename", ".ext", "2016_07_07 18_56_08 UTC", created);

            var expected = new DateTime(2016, 7, 7, 18, 56, 8);
            Assert.AreEqual(expected, target.CreationTime);
        }

        [TestMethod]
        public void Ctor_FileSystemNull_ThrowsArgumentNullException()
        {
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryFile(null, @"path", "name", ".ext", "timestamp", created));
        }

        [TestMethod]
        public void Ctor_PathNull_ThrowsArgumentNullException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryFile(mockFileSystem, null, "name", ".ext", "timestamp", created));
        }

        [TestMethod]
        public void Ctor_PathEmpty_ThrowsArgumentException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentException>(() => new FileHistoryFile(mockFileSystem, "", "name", ".ext", "timestamp", created));
        }

        [TestMethod]
        public void Ctor_NameNull_ThrowsArgumentNullException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryFile(mockFileSystem, "path", null, ".ext", "timestamp", created));
        }

        [TestMethod]
        public void Ctor_NameEmpty_ThrowsArgumentException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentException>(() => new FileHistoryFile(mockFileSystem, "path", "", ".ext", "timestamp", created));
        }

        [TestMethod]
        public void Ctor_ExtensionNull_ThrowsArgumentNullException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryFile(mockFileSystem, "path", "name", null, "timestamp", created));
        }

        [TestMethod]
        public void Ctor_TimestampNull_ThrowsArgumentNullException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryFile(mockFileSystem, "path", "name", ".ext", null, created));
        }

        [TestMethod]
        public void Ctor_TimestampEmpty_ThrowsArgumentException()
        {
            var mockFileSystem = new MockFileSystem();
            var created = DateTime.UtcNow;

            Assert.ThrowsException<ArgumentException>(() => new FileHistoryFile(mockFileSystem, "path", "name", ".ext", "", created));
        }
    }
}
