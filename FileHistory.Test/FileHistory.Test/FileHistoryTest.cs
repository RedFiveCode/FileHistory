using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FileHistory.Core;
using System.IO.Abstractions.TestingHelpers;
using System.Collections.Generic;

namespace FileHistory.Test
{
    [TestClass]
    public class FileHistoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFolderGroupDetails_Path_Null_Throws_ArgumentNullException()
        {
            var target = new Core.FileHistory();

            var results = target.GetFolderGroupDetails(null, false, String.Empty, 0L);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFolderGroupDetails_Path_Empty_Throws_ArgumentNullException()
        {
            var target = new Core.FileHistory();

            var results = target.GetFolderGroupDetails("", false, String.Empty, 0L);
        }

        [TestMethod]
        public void GetFolderGroupDetails_OneGroup_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            var target = new Core.FileHistory(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath");

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            AssertGroup(results.First(), "filename.ext", 3);
        }

        [TestMethod]
        public void GetFolderGroupDetails_TwoGroups_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new Core.FileHistory(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath");

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            AssertGroup(results[0], "another filename.extension", 2);
            AssertGroup(results[1], "filename.ext", 3);
        }

        [TestMethod]
        public void GetFolderGroupDetails_Wildcard_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new Core.FileHistory(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "an*.*", 0L);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            AssertGroup(results[0], "another filename.extension", 2);
        }

        [TestMethod]
        public void GetFolderGroupDetails_WildcardNoMatch_Returns_EmptyListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new Core.FileHistory(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "Z*.*", 0L);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetFolderGroupDetails_Recursive_Returns_ListOfFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new Core.FileHistory(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", true, "*.*", 0L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            AssertGroup(results[0], @"\\server\somepath\FolderTwo", "another filename.extension", 2);
            AssertGroup(results[1], @"\\server\somepath\FolderOne", "filename.ext", 3);
        }

        [TestMethod]
        public void GetFolderGroupDetails_LargeFiles_Returns_ListOfFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13), 1000);
            AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14), 1000);

            var target = new Core.FileHistory(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "*.*", 100L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            AssertGroup(results[0], @"\\server\somepath", "another filename.extension", 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFileDetails_Path_Null_Throws_ArgumentNullException()
        {
            var target = new Core.FileHistory();

            target.GetFileDetails(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFileDetails_Path_Empty_Throws_ArgumentNullException()
        {
            var target = new Core.FileHistory();

            target.GetFileDetails("");
        }

        [TestMethod]
        public void GetFileDetails_FileWithExtension_Returns_ValidObject()
        {
            var target = new Core.FileHistory();

            var result = target.GetFileDetails(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext");

            Assert.IsNotNull(result);

            Assert.AreEqual(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", result.FullPath);
            Assert.AreEqual("filename", result.Name); 
            Assert.AreEqual("2016_07_07 18_56_08 UTC", result.Time);
            Assert.AreEqual(".ext", result.Extension);
        }

        [TestMethod]
        public void GetFileDetails_FileWithoutExtension_Returns_ValidObject()
        {
            var target = new Core.FileHistory();

            var result = target.GetFileDetails(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC)");

            Assert.IsNotNull(result);

            Assert.AreEqual(@"\\server\somepath\filename (2016_07_07 18_56_08 UTC)", result.FullPath);
            Assert.AreEqual("filename", result.Name);
            Assert.AreEqual("2016_07_07 18_56_08 UTC", result.Time);
            Assert.AreEqual(String.Empty, result.Extension);
        }

        [TestMethod]
        public void GetFileDetails_FileNotMatched_Returns_Null()
        {
            var target = new Core.FileHistory();

            var result = target.GetFileDetails(@"\\server\somepath\filename.ext");

            Assert.IsNull(result);
        }

        private void AddToFileSystem(MockFileSystem mockFileSystem, string path, DateTime created)
        {
            mockFileSystem.AddFile(path, new MockFileData("Test data...") { CreationTime = created });
        }

        private void AddToFileSystem(MockFileSystem mockFileSystem, string path, DateTime created, int length)
        {
            var data = new string('.', length);
            mockFileSystem.AddFile(path, new MockFileData(data) { CreationTime = created });
        }

        private void AssertContainsFile(IEnumerable<FileHistoryGroup> files, string filename)
        {
            Assert.IsTrue(files.Any(fhg => fhg.Fullname == filename), $"File '{filename}' not found in file history groups");
        }

        private void AssertGroup(FileHistoryGroup group, string expectedFilename, int expectedCount)
        {
            Assert.IsNotNull(group);
            Assert.AreEqual(expectedFilename, group.Fullname);
            Assert.AreEqual(expectedCount, group.FileCount);
        }

        private void AssertGroup(FileHistoryGroup group, string expectedFoldername, string expectedFilename, int expectedCount)
        {
            Assert.IsNotNull(group);
            Assert.AreEqual(expectedFoldername, group.Folder);
            Assert.AreEqual(expectedFilename, group.Fullname);
            Assert.AreEqual(expectedCount, group.FileCount);
        }
    }
}
