using FileHistory.Core;
using System;
using System.IO.Abstractions.TestingHelpers;

public class MockFileSystemHelper
{
    public static void AddToFileSystem(MockFileSystem mockFileSystem, string path, DateTime created)
    {
        mockFileSystem.AddFile(path, new MockFileData("Test data...") { CreationTime = created });
    }

    public static void AddToFileSystem(MockFileSystem mockFileSystem, string path, DateTime created, int length)
    {
        var data = new string('.', length);
        mockFileSystem.AddFile(path, new MockFileData(data) { CreationTime = created });
    }

    public static FileHistoryFile CreateFileHistoryFileAndAddToFileSystem(MockFileSystem mockFileSystem, string path, string filename, string extension, DateTime created)
    {
        var timestamp = $"{created:yyyy_MM_dd HH_mm_ss} UTC";
        var fullPath = $"{path}\\{filename} ({created:yyyy_MM_dd HH_mm_ss} UTC){extension}";

        mockFileSystem.AddFile(fullPath, new MockFileData("Test data...") { CreationTime = created });

        return new FileHistoryFile(mockFileSystem, fullPath, filename, extension, timestamp, created);
    }
}
