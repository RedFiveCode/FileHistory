using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using FileHistory.Core;
using System.IO.Abstractions.TestingHelpers;

namespace FileHistory.Test
{
    [TestClass]
    public class FileHistoryDiscoveryTest
    {
        [TestMethod]
        public void IsMatchingFile_FilenameNull_Returns_False()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var result = target.IsMatchingFile(null);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMatchingFile_FilenameFullPathValid_Returns_True()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var result = target.IsMatchingFile(@"\\server\somepath\filename (2021_01_01 10_10_00 UTC).extension");

            // assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFolderGroupDetails_Path_Null_Throws_ArgumentNullException()
        {
            var target = new FileHistoryDiscovery();

            var results = target.GetFolderGroupDetails(null, false, String.Empty, 0L);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFolderGroupDetails_Path_Empty_Throws_ArgumentException()
        {
            var target = new FileHistoryDiscovery();

            var results = target.GetFolderGroupDetails("", false, String.Empty, 0L);
        }

        [TestMethod]
        public void GetFolderGroupDetails_OneGroup_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            var target = new FileHistoryDiscovery(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath");

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            AssertGroup(results.First(), @"\\server\somepath", "filename.ext", 3);
        }

        [TestMethod]
        public void GetFolderGroupDetails_TwoGroups_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new FileHistoryDiscovery(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath");

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            AssertGroup(results[0], @"\\server\somepath", "another filename.extension", 2);
            AssertGroup(results[1], @"\\server\somepath", "filename.ext", 3);
        }

        [TestMethod]
        public void GetFolderGroupDetails_Wildcard_Returns_ListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new FileHistoryDiscovery(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "an*.*", 0L);

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            AssertGroup(results[0], @"\\server\somepath", "another filename.extension", 2);
        }

        [TestMethod]
        public void GetFolderGroupDetails_WildcardNoMatch_Returns_EmptyListOfFiles()
        {
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new FileHistoryDiscovery(mockFileSystem);

            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "Z*.*", 0L);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetFolderGroupDetails_Recursive_Returns_ListOfFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14));

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", true, "*.*", 0L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            AssertGroup(results[0], @"\\server\somepath\FolderOne", "filename.ext", 3);
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_10_00 UTC).ext");
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_11_00 UTC).ext");
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_12_00 UTC).ext");

            AssertGroup(results[1], @"\\server\somepath\FolderTwo", "another filename.extension", 2);
            AssertGroupContainsFile(results[1], "another filename (2021_01_01 13_13_13 UTC).extension");
            AssertGroupContainsFile(results[1], "another filename (2021_01_01 14_14_14 UTC).extension");
        }

        [TestMethod]
        public void GetFolderGroupDetails_Recursive_SameFilenameInDifferentSubFolders_Returns_ListOfFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\filename (2021_02_02 20_13_13 UTC).ext", new DateTime(2021, 2, 2, 20, 13, 13));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo\filename (2021_02_02 21_14_14 UTC).ext", new DateTime(2021, 2, 2, 21, 14, 14));

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", true, "*.*", 0L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);

            AssertGroup(results[0], @"\\server\somepath\FolderOne", "filename.ext", 3);
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_10_00 UTC).ext");
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_11_00 UTC).ext");
            AssertGroupContainsFile(results[0], "filename (2021_01_01 10_12_00 UTC).ext");

            AssertGroup(results[1], @"\\server\somepath\FolderTwo", "filename.ext", 2);
            AssertGroupContainsFile(results[1], "filename (2021_02_02 20_13_13 UTC).ext");
            AssertGroupContainsFile(results[1], "filename (2021_02_02 21_14_14 UTC).ext");
        }

        [TestMethod]
        public void GetFolderGroupDetails_LargeFiles_Returns_ListOfFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13), 1000);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14), 1000);

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "*.*", 100L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            AssertGroup(results[0], @"\\server\somepath", "another filename.extension", 2);
        }

        [TestMethod]
        public void GetFolderGroupDetails_NoMatchingLargeFiles_Returns_EmptyList()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13), 1000);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14), 1000);

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", false, "*.*", 1000 * 1000L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetFolderGroupDetails_NoMatchingFiles_Returns_EmptyList()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 13_13_13 UTC).extension", new DateTime(2021, 1, 1, 13, 13, 13), 1000);
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 14_14_14 UTC).extension", new DateTime(2021, 1, 1, 14, 14, 14), 1000);

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath", true, "*.myextension", 0L);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetFolderGroupDetails_Ignores_NonFileHistoryFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\some random file", new DateTime(2021, 1, 1, 1, 1, 1));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\another random file.ext", new DateTime(2021, 1, 1, 1, 1, 1));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_10_00 UTC).ext", new DateTime(2021, 1, 1, 10, 10, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_11_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));
            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2021_01_01 10_12_00 UTC).ext", new DateTime(2021, 1, 1, 10, 12, 0));

            var target = new FileHistoryDiscovery(mockFileSystem);

            // act
            var results = target.GetFolderGroupDetails(@"\\server\somepath");

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            AssertGroup(results[0], @"\\server\somepath", "filename.ext", 3);
            Assert.IsFalse(results[0].Files.Any(fh => fh.FileName == "some random file"));
            Assert.IsFalse(results[0].Files.Any(fh => fh.FileName == "another random file.ext"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFileDetails_Path_Null_Throws_ArgumentNullException()
        {
            var target = new FileHistoryDiscovery();

            target.GetFileDetails(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileDetails_Path_Empty_Throws_ArgumentException()
        {
            var target = new FileHistoryDiscovery();

            target.GetFileDetails("");
        }

        [TestMethod]
        public void GetFileDetails_FileWithExtension_Returns_ValidObject()
        {
            var mockFileSystem = new MockFileSystem();

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC).ext", new DateTime(2016, 7, 7, 18, 56, 8));

            var target = new FileHistoryDiscovery(mockFileSystem);

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
            var mockFileSystem = new MockFileSystem();

            MockFileSystemHelper.AddToFileSystem(mockFileSystem, @"\\server\somepath\filename (2016_07_07 18_56_08 UTC)", new DateTime(2016, 7, 7, 18, 56, 8));

            var target = new FileHistoryDiscovery(mockFileSystem);

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
            var target = new FileHistoryDiscovery();

            var result = target.GetFileDetails(@"\\server\somepath\filename.ext");

            Assert.IsNull(result);
        }

        private void AssertGroup(FileHistoryGroup group, string expectedFoldername, string expectedFilename, int expectedCount)
        {
            Assert.IsNotNull(group);
            Assert.AreEqual(expectedFoldername, group.Folder);
            Assert.AreEqual(expectedFilename, group.Fullname);
            Assert.AreEqual(expectedCount, group.FileCount);
        }

        private void AssertGroupContainsFile(FileHistoryGroup group, string expectedFilename)
        {
            Assert.IsNotNull(group);
            Assert.IsTrue(group.Files.Any(fh => fh.RawName == expectedFilename));
        }
    }
}
