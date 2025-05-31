using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace FileHistory.Test
{
    /// <summary>
    /// Unit tests for the <see cref="FileHistoryGroup"/> class
    /// </summary>
    [TestClass]
    public class FileHistoryGroupTest
    {
        [TestMethod]
        public void FileHistoryGroup_Ctor_Sets_Properties()
        {
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(@"\\server\somepath", target.Folder);
            Assert.AreEqual("filename.ext", target.Fullname);
            Assert.AreEqual("filename", target.Name);
            Assert.AreEqual(".ext", target.Extension);
            Assert.IsNotNull(target.Files);
            CollectionAssert.AreEqual(fhList, target.Files);
        }

        [TestMethod]
        public void FileCount_Returns_NumberOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(fhList.Count, target.FileCount);
        }

        [TestMethod]
        public void FileSize_Returns_TotalFileSize()
        {
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\filename", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\filename", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\filename", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup("filename.ext", fhList);

            Assert.AreEqual(36L, target.FileSize); // 3 files each with 12 bytes ("Test data...")
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetFilesToKeepAndDelete_RecentGenerationsToKeepLessThanZero_Throws_ArgumentOutOfRangeException()
        {
            var target = new FileHistoryGroup(@"\\server\somepath\filename.ext", new List<FileHistoryFile>());

            target.GetFilesToKeepAndDelete(-1);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_EmptyGroup_Returns_EmptyLists()
        {
            // arrange
            var target = new FileHistoryGroup(@"\\server\somepath\filename.ext", new List<FileHistoryFile>());

            // act
            var result = target.GetFilesToKeepAndDelete(1);

            // assert
            Assert.IsNotNull(result);

            Assert.IsNotNull(result.KeepList);
            Assert.AreEqual(0, result.KeepList.Count);

            Assert.IsNotNull(result.DeleteList);
            Assert.AreEqual(0, result.DeleteList.Count);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_KeepFewerGenerationsThanAvailableFiles_ReturnsKeepListAndDeleteList()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup(@"\\server\somepath", fhList);


            // act
            var result = target.GetFilesToKeepAndDelete(1);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.KeepList);
            Assert.IsNotNull(result.DeleteList);

            Assert.AreEqual(target.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

            // keep list
            Assert.AreEqual(1, result.KeepList.Count);
            AssertContainsFile(result.KeepList, @"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext"); // most recent file

            // delete list
            Assert.AreEqual(2, result.DeleteList.Count);
            AssertContainsFile(result.DeleteList, @"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext"); // earlier files
            AssertContainsFile(result.DeleteList, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_ReturnsKeepListAndDeleteList_WithAllItems()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(1);

            // assert
            Assert.AreEqual(target.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

            var resultList = new List<FileHistoryFile>();
            resultList.AddRange(result.KeepList);
            resultList.AddRange(result.DeleteList);
            CollectionAssert.AreEquivalent(fhList, resultList);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_KeepOneGeneration_ReturnsKeepListWithMostRecentFile_And_DeleteListWithOlderFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var target = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(1);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.KeepList);
            Assert.IsNotNull(result.DeleteList);

            Assert.AreEqual(target.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

            // keep list
            Assert.AreEqual(fhList.Select(fh => fh.CreationTime).Max(), result.KeepList.First().CreationTime);

            // delete list
            Assert.AreEqual(fhList.Select(fh => fh.CreationTime).Min(), result.DeleteList.Last().CreationTime);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_KeepMoreGenerationsThanAvailableFiles_ReturnsKeepListWithAllFiles_And_DeleteListIsEmpty()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 4)),
            };

            var target = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(99); // attempt to keep more generations (99) than available files (4)

            // assert

            // keep list - all items should be included
            Assert.AreEqual(target.FileCount, result.KeepList.Count);
            CollectionAssert.AreEquivalent(fhList, new List<FileHistoryFile>(result.KeepList));

            // delete list - no items should be excluded
            Assert.AreEqual(0, result.DeleteList.Count);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_ReturnsKeepListAndDeleteListOrderedByCreationTime()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 4))
            };


            var target = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(2);

            // keep list
            Assert.AreEqual(2, result.KeepList.Count);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_04 00_00_00 UTC).ext", result.KeepList[0].FullPath); // most recent files
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext", result.KeepList[1].FullPath);

            // delete list
            Assert.AreEqual(2, result.DeleteList.Count);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext", result.DeleteList[0].FullPath);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext", result.DeleteList[1].FullPath);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_OnlyFile_ReturnsKeepListAndDeleteList()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
            };


            var target = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(1);

            // keep list
            Assert.AreEqual(1, result.KeepList.Count);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext", result.KeepList[0].FullPath); // most recent files

            // delete list
            Assert.AreEqual(0, result.DeleteList.Count);
        }

        [TestMethod]
        public void Ctor_FullnameNull_ThrowsArgumentNullException()
        {
            var mockFileSystem = new MockFileSystem();
            var files = new List<FileHistoryFile>();
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryGroup(null, files));
        }

        [TestMethod]
        public void Ctor_FullnameEmpty_ThrowsArgumentException()
        {
            var mockFileSystem = new MockFileSystem();
            var files = new List<FileHistoryFile>();
            Assert.ThrowsException<ArgumentException>(() => new FileHistoryGroup("", files));
        }

        [TestMethod]
        public void Ctor_FilesNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new FileHistoryGroup("filename.ext", null));
        }

        private void AssertContainsFile(IEnumerable<FileHistoryFile> files, string filename)
        {
            Assert.IsTrue(files.Any(fh => fh.FullPath == filename), $"File '{filename}' not found");
        }
    }
}
