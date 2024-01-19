using Krake.Cli.Features.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krake.Cli.Features.FinanceData;

public static class FinanceDataServiceCollectionExtensions
{
    private const string Key = "finance-data";
    public static IServiceCollection AddFinanceDataModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddKeyedSingleton(Key, new DirectoryManager(config["Apps:FinanceData:RootDirectory"]!));
        services.AddKeyedSingleton<FinanceDataFileManager>(Key);
        services.AddKeyedSingleton<IImporterApplication, FinanceDataImporterApp>(Key);
        return services;
    }
}