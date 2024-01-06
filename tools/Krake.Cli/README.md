# Krake.Cli Tool

## Introduction

First, you add the following three lines to the `Krake.Cli.csproj`.

```csharp
<PackAsTool>true</PackAsTool>
<ToolCommandName>krake</ToolCommandName>
<PackageOutputPath>./nupkg</PackageOutputPath>
```

Second, you pack the cli tool as a nuget package via the terminal in the root directory.

```csharp
dotnet pack
```

Third, you install the tool globally on your PC.

```csharp
dotnet tool install -g --add-source ./nupkg Krake.Cli
```

Finally, you can run the tool globally from any terminal.

```
krake ...
```

If you need to uninstall the tool globally the you can run the following command.

```csharp
dotnet tool uninstall -g Krake.Cli
```