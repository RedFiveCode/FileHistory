using FileHistory.App;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace FileHistory.Test
{
    [TestClass]
    public class TidyOperationTest
    {
        [TestMethod]
        public void TidyFolder_DeletesMatchingFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath");
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3));

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 2));


            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath",
                RecurseSubFolders = false,
                KeepGenerations = 1,
                Preview = false,
                WildcardFilter = "*.*"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert
            // should keep newest generation of each file, and delete older generation(s) of each file
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");

            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\another filename (2021_02_01 00_00_00 UTC).ext");
        }

        [TestMethod]
        public void TidyFolder_KeepsMatchingFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath");
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3));

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 2));

            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath",
                RecurseSubFolders = false,
                KeepGenerations = 1,
                Preview = false,
                WildcardFilter = "*.*"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert
            // should keep newest generation of each file, and delete older generation(s) of each file

            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_02_02 00_00_00 UTC).ext");
        }

        [TestMethod]
        public void TidyFolder_Preview_DoesNotDeleteFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath\FolderOne");
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3));

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".ext", new DateTime(2021, 2, 2));

            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath\FolderOne",
                RecurseSubFolders = false,
                KeepGenerations = 1,
                Preview = true,
                WildcardFilter = "*.*"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert - should keep all files
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext");

            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_02_01 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_02_02 00_00_00 UTC).ext");
        }

        [TestMethod]
        public void TidyFolder_KeepGenerations_KeepsRecentFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath");
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 4));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 5));

            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath",
                RecurseSubFolders = false,
                KeepGenerations = 2,
                Preview = false,
                WildcardFilter = "*.*"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert - should keep 2 most recent files
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_05 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_04 00_00_00 UTC).ext");
            
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");
        }

        [TestMethod]
        public void TidyFolder_Wildcard_DeletesMatchingFiles_And_KeepsNonMatchingFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath");
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "filename", ".ext", new DateTime(2021, 1, 3));

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".dat", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".dat", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath", "another filename", ".dat", new DateTime(2021, 1, 3));
                                                                                                                
            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath",
                RecurseSubFolders = false,
                KeepGenerations = 1,
                Preview = false,
                WildcardFilter = "*.ext"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert - should keep 1 most recent files
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\filename (2021_01_03 00_00_00 UTC).ext");

            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_02 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\filename (2021_01_01 00_00_00 UTC).ext");

            // and keep all the .dat files
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_01_01 00_00_00 UTC).dat");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_01_02 00_00_00 UTC).dat");
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\another filename (2021_01_03 00_00_00 UTC).dat");
        }

        [TestMethod]
        public void TidyFolder_Recursive_DeletesMatchingFiles()
        {
            // arrange
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(@"\\server\somepath\FolderOne");
            mockFileSystem.AddDirectory(@"\\server\somepath\FolderTwo");

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne", "filename", ".ext", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne", "filename", ".ext", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderOne", "filename", ".ext", new DateTime(2021, 1, 3));

            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo", "another filename", ".dat", new DateTime(2021, 1, 1));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo", "another filename", ".dat", new DateTime(2021, 1, 2));
            MockFileSystemHelper.CreateFileHistoryFileAndAddToFileSystem(mockFileSystem, @"\\server\somepath\FolderTwo", "another filename", ".dat", new DateTime(2021, 1, 3));

            var options = new TidyCommandLineOptions()
            {
                Folder = @"\\server\somepath",
                RecurseSubFolders = true,
                KeepGenerations = 1,
                Preview = false,
                WildcardFilter = "*.*"
            };
            var target = new TidyOperation(mockFileSystem);

            // act
            target.TidyFolder(options);

            // assert

            // FolderOne
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_03 00_00_00 UTC).ext"); // most recent
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_02 00_00_00 UTC).ext");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\FolderOne\filename (2021_01_01 00_00_00 UTC).ext");

            // FolderTwo
            AssertMockFileSystem.AssertFileExists(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_03 00_00_00 UTC).dat"); // most recent
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_02 00_00_00 UTC).dat");
            AssertMockFileSystem.AssertFileNotExist(mockFileSystem, @"\\server\somepath\FolderTwo\another filename (2021_01_01 00_00_00 UTC).dat");
        }
    }
}
