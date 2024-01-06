using System.IO.Compression;
using Krake.Core;

namespace Krake.Cli.Features;

public sealed class DirectoryManager(string rootDirectory)
{
    private const string InFolder = "In";
    private const string OutFolder = "Out";
    private const string ArchiveFolder = "Archive";
    private const string ProcessedFolder = "Processed";
    private const string FailedFolder = "Failed";

    public DirectoryInfo Root { get; } = CreateDirectory(rootDirectory);
    public DirectoryInfo In { get; } = CreateDirectory(Path.Combine(rootDirectory, InFolder));
    public DirectoryInfo Out { get; } = CreateDirectory(Path.Combine(rootDirectory, OutFolder));
    public DirectoryInfo Archive { get; } = CreateDirectory(Path.Combine(rootDirectory, ArchiveFolder));
    public DirectoryInfo Processed { get; } = CreateDirectory(Path.Combine(rootDirectory, ProcessedFolder));
    public DirectoryInfo Failed { get; } = CreateDirectory(Path.Combine(rootDirectory, FailedFolder));

    public static DirectoryInfo CreateRelativeDirectory(string rootPath, string relativePath) =>
        CreateDirectory(Path.Combine(rootPath, relativePath));

    public static DirectoryInfo CreateAbsoluteDirectory(string absolutePath) =>
        CreateDirectory(absolutePath);

    public Result<Error, FileInfo> ZipOutDirectoryToArchive(string zipFileName) =>
        CreateZipFileFromDirectory(zipFileName, Out, Archive);

    public Result<Error, FileInfo> ZipInDirectoryToArchive(string zipFileName) =>
        CreateZipFileFromDirectory(zipFileName, In, Archive);

    public Result<Error, Success> MoveFileToProcessed(FileInfo fileInfo, string zipFileName) =>
        AddFileToZipArchive(fileInfo, CreateFile(Processed, zipFileName));

    public Result<Error, Success> MoveFileToFailed(FileInfo fileInfo, string zipFileName) =>
        AddFileToZipArchive(fileInfo, CreateFile(Failed, zipFileName));

    private static DirectoryInfo CreateDirectory(string path) =>
        Directory.Exists(path) is false ? Directory.CreateDirectory(path) : new DirectoryInfo(path);

    private static FileInfo CreateFile(FileSystemInfo destinationDirectory, string fileName) =>
        new(Path.Combine(destinationDirectory.FullName, fileName));

    private static FileInfo CreateZipFileFromDirectory(string zipFileName, FileSystemInfo fromDirectory,
        FileSystemInfo toDirectory, DirectoryInfo? tempDirectory = null)
    {
        var temp = tempDirectory ?? Directory.CreateTempSubdirectory();
        var zipFile = CreateFile(temp, zipFileName);
        var destinationFile = CreateFile(toDirectory, zipFileName);

        ZipFile.CreateFromDirectory(fromDirectory.FullName, zipFile.FullName);

        if (File.Exists(destinationFile.FullName))
        {
            File.Delete(destinationFile.FullName);
        }

        zipFile.MoveTo(destinationFile.FullName);
        temp.Delete();

        return destinationFile;
    }

    private static Success AddFileToZipArchive(FileSystemInfo fileInfo, FileSystemInfo zipFile)
    {
        using var zipToOpenOrCreate = new FileStream(zipFile.FullName, FileMode.OpenOrCreate);
        using var archive = new ZipArchive(zipToOpenOrCreate, ZipArchiveMode.Update);
        var entry = archive.GetEntry(fileInfo.Name);
        if (entry is null)
        {
            _ = archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
            return Ok.Success;
        }

        if (fileInfo.LastWriteTimeUtc <= entry.LastWriteTime)
        {
            return Ok.Success;
        }

        entry.Delete();
        _ = archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
        return Ok.Success;
    }
}