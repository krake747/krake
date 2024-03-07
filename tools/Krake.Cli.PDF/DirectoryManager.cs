namespace Krake.Cli.PDF;

public sealed class DirectoryManager(string rootDirectory)
{
    private const string InFolder = "In";
    private const string OutFolder = "Out";

    public DirectoryInfo Root { get; } = CreateDirectory(rootDirectory);
    public DirectoryInfo In { get; } = CreateDirectory(Path.Combine(rootDirectory, InFolder));
    public DirectoryInfo Out { get; } = CreateDirectory(Path.Combine(rootDirectory, OutFolder));

    private static DirectoryInfo CreateDirectory(string path) =>
        Directory.Exists(path) is false ? Directory.CreateDirectory(path) : new DirectoryInfo(path);
}