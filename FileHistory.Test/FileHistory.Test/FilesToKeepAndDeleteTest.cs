using FileHistory.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FileHistory.Test
{
    [TestClass]
    public class FilesToKeepAndDeleteTest
    {
        [TestMethod]
        public void Ctor_Sets_KeepList_And_DeleteList_Properties()
        {
            // Arrange
            var keepList = new List<FileHistoryFile>
            {
                CreateFileHistoryFile("keep1"),
                CreateFileHistoryFile("keep2")
            };
            var deleteList = new List<FileHistoryFile>
            {
                CreateFileHistoryFile("delete1")
            };

            // Act
            var target = new FilesToKeepAndDelete(keepList, deleteList);

            // Assert
            CollectionAssert.AreEqual(keepList, (System.Collections.ICollection)target.KeepList);
            CollectionAssert.AreEqual(deleteList, (System.Collections.ICollection)target.DeleteList);
        }

        [TestMethod]
        public void Ctor_Copies_Lists_Defensively()
        {
            // Arrange
            var keepList = new List<FileHistoryFile>
            {
                CreateFileHistoryFile("keep1")
            };
            var deleteList = new List<FileHistoryFile>
            {
                CreateFileHistoryFile("delete1")
            };
            var target = new FilesToKeepAndDelete(keepList, deleteList);

            // Act
            keepList.Add(CreateFileHistoryFile("keep2"));
            deleteList.Clear();

            // Assert
            Assert.AreEqual(1, target.KeepList.Count);
            Assert.AreEqual(1, target.DeleteList.Count);

            Assert.AreNotSame(keepList, target.KeepList);
            Assert.AreNotSame(deleteList, target.DeleteList);
        }

        [TestMethod]
        public void KeepList_IsNotNull()
        {
            // Arrange
            var keepList = new List<FileHistoryFile>();
            var deleteList = new List<FileHistoryFile>();

            // Act
            var target = new FilesToKeepAndDelete(keepList, deleteList);

            // Assert
            Assert.IsNotNull(target.KeepList);
        }

        [TestMethod]
        public void DeleteList_IsNotNull()
        {
            // Arrange
            var keepList = new List<FileHistoryFile>();
            var deleteList = new List<FileHistoryFile>();

            // Act
            var target = new FilesToKeepAndDelete(keepList, deleteList);

            // Assert
            Assert.IsNotNull(target.DeleteList);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Ctor_KeepListNull_DeleteListNull_ThrowsArgumentNullException(bool keepListNull, bool deleteListNull)
        {
            // Arrange
           var keepList = keepListNull ? null : new List<FileHistoryFile>();
           var deleteList = deleteListNull ? null : new List<FileHistoryFile>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new FilesToKeepAndDelete(keepList, deleteList));
        }

        private static FileHistoryFile CreateFileHistoryFile(string name)
        {
            // Minimal stub for testing; adjust as needed for your constructor
            var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            var path = $@"C:\rootFolder\{name}.txt";
            fileSystem.AddFile(path, new System.IO.Abstractions.TestingHelpers.MockFileData("data"));
            
            return new FileHistoryFile(fileSystem, path, name, ".txt", "2021_01_01 00_00_00 UTC", new DateTime(2021, 1, 1));
        }
    }
}
