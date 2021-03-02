using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace FileHistory.Test
{
    [TestClass]
    public class TidyFilterTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilesToKeepAndDelete_FileGroupsNull_Throws_ArgumentNullException()
        {
            var target = new TidyFilter();

            target.GetFilesToKeepAndDelete(null, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetFilesToKeepAndDelete_RecentGenerationsToKeepLessThanZero_Throws_ArgumentOutOfRangeException()
        {
            var target = new TidyFilter();
            var group = new FileHistoryGroup(@"\\server\somepath\filename.ext", new List<FileHistoryFile>());

            target.GetFilesToKeepAndDelete(group, -1);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_EmptyGroup_Returns_EmptyLists()
        {
            // arrange
            var target = new TidyFilter();
            var group = new FileHistoryGroup(@"\\server\somepath\filename.ext",  new List<FileHistoryFile>());

            // act
            var result = target.GetFilesToKeepAndDelete(group, 1);

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
            var target = new TidyFilter();

            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var group = new FileHistoryGroup(@"\\server\somepath", fhList);


            // act
            var result = target.GetFilesToKeepAndDelete(group, 1);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.KeepList);
            Assert.IsNotNull(result.DeleteList);

            Assert.AreEqual(group.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

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
            var target = new TidyFilter();

            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var group = new FileHistoryGroup(@"\\server\somepath", fhList);


            // act
            var result = target.GetFilesToKeepAndDelete(group, 1);

            // assert
            Assert.AreEqual(group.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

            var resultList = new List<FileHistoryFile>();
            resultList.AddRange(result.KeepList);
            resultList.AddRange(result.DeleteList);
            CollectionAssert.AreEquivalent(fhList, resultList);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_KeepOneGeneration_ReturnsKeepListWithMostRecentFile_And_DeleteListWithOlderFiles()
        {
            // arrange
            var target = new TidyFilter();

            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
            };

            var group = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(group, 1);

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.KeepList);
            Assert.IsNotNull(result.DeleteList);

            Assert.AreEqual(group.FileCount, (result.KeepList.Count + result.DeleteList.Count)); // all items are included

            // keep list
            Assert.AreEqual(fhList.Select(fh => fh.CreationTime).Max(), result.KeepList.First().CreationTime);

            // delete list
            Assert.AreEqual(fhList.Select(fh => fh.CreationTime).Min(), result.DeleteList.Last().CreationTime);
        }

        [TestMethod]
        public void GetFilesToKeepAndDelete_KeepMoreGenerationsThanAvailableFiles_ReturnsKeepListWithAllFiles_And_DeleteListIsEmpty()
        {
            // arrange
            var target = new TidyFilter();

            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 4)),
            };

            var group = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(group, 99); // attempt to keep more generations (99) than available files (4)

            // assert

            // keep list - all items should be included
            Assert.AreEqual(group.FileCount, result.KeepList.Count);
            CollectionAssert.AreEquivalent(fhList, new List<FileHistoryFile>(result.KeepList));

            // delete list - no items should be excluded
            Assert.AreEqual(0, result.DeleteList.Count);
        }
                
        [TestMethod]
        public void GetFilesToKeepAndDelete_ReturnsKeepListAndDeleteListOrderedByCreationTime()
        {
            // arrange
            var target = new TidyFilter();

            var mockFileSystem = new MockFileSystem();
            var fhList = new List<FileHistoryFile>()
            {
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3)),
                CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 4))
            };


            var group = new FileHistoryGroup(@"\\server\somepath", fhList);

            // act
            var result = target.GetFilesToKeepAndDelete(group, 2);

            // keep list
            Assert.AreEqual(2, result.KeepList.Count);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_04 00_00_00 UTC).ext", result.KeepList[0].FullPath); // most recent files
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext", result.KeepList[1].FullPath);

            // delete list
            Assert.AreEqual(2, result.DeleteList.Count);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext", result.DeleteList[0].FullPath);
            Assert.AreEqual(@"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext", result.DeleteList[1].FullPath);
        }

        private void AddToFileSystem(MockFileSystem mockFileSystem, string path, DateTime created)
        {
            mockFileSystem.AddFile(path, new MockFileData("Test data...") { CreationTime = created });
        }


        private List<FileHistoryFile> CreateFileList(string folder, string filename, int fileCount)
        {
            var list = new List<FileHistoryFile>();

            var timestamp = new DateTime(2020, 12, 31);

            for (int n = 0; n < fileCount; n++)
            {
                timestamp = timestamp.AddDays(1);
                var formattedTimestamp = $"{0:yyyy_MM_dd HH_mm_ss} UTC";

                var ext = ".ext";
                var path = $"\\\\{folder}\\{filename} ({formattedTimestamp}){ext}";

                var fhf = new FileHistoryFile(path, filename, ext, formattedTimestamp);

                list.Add(fhf);
            }

            return list;
        }

        private FileHistoryFile CreateFileHistoryFileAndAddToFileSystem(MockFileSystem mockFileSystem, string path, string filename, string extension, DateTime created)
        {
            var timestamp = $"{created:yyyy_MM_dd HH_mm_ss} UTC";
            var fullPath = $"{path}\\{filename} ({created:yyyy_MM_dd HH_mm_ss} UTC){extension}";

            mockFileSystem.AddFile(fullPath, new MockFileData("Test data...") { CreationTime = created });

            return new FileHistoryFile(mockFileSystem, fullPath, filename, extension, timestamp);
        }


        private void AssertContainsFile(IEnumerable<FileHistoryFile> files, string filename)
        {
            Assert.IsTrue(files.Any(fh => fh.FullPath == filename), $"File '{filename}' not found");
        }


    }
}
