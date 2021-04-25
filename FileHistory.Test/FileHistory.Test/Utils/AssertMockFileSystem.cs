using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Abstractions.TestingHelpers;

public class AssertMockFileSystem
{ 
    public static void AssertFileExists(MockFileSystem mockFileSystem, string filename)
    {
        var exists = mockFileSystem.FileExists(filename);

        Assert.IsTrue(exists, $"File ${filename} has been deleted when should exist");
    }

    public static void AssertFileNotExist(MockFileSystem mockFileSystem, string filename)
    {
        var exists = mockFileSystem.FileExists(filename);

        Assert.IsFalse(exists, $"File ${filename} exists when should be deleted");
    }
}
