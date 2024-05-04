using System.Text;

namespace Krake.Api.Swagger;

internal static class SwashbuckleSchemaHelper
{
    private static readonly Dictionary<string, List<string>> SchemaNameRepetition = [];

    private static string DefaultSchemaIdSelector(Type modelType)
    {
        if (modelType.IsConstructedGenericType is false)
        {
            return modelType.Name.Replace("[]", "Array", StringComparison.InvariantCulture);
        }

        var prefix = modelType.GetGenericArguments()
            .Select(DefaultSchemaIdSelector)
            .Aggregate(new StringBuilder(), (sb, current) => sb.Append(current))
            .ToString();

        return $"{prefix}{modelType.Name.Split('`')[0]}";
    }

    public static string GetSchemaId(Type modelType)
    {
        var id = DefaultSchemaIdSelector(modelType);

        if (SchemaNameRepetition.ContainsKey(id) is false)
        {
            SchemaNameRepetition.Add(id, []);
        }

        var modelNameList = SchemaNameRepetition[id];
        var fullName = modelType.FullName ?? string.Empty;
        if (string.IsNullOrEmpty(fullName) is false && modelNameList.Contains(fullName) is false)
        {
            modelNameList.Add(fullName);
        }

        var namespaces = fullName.Split('.');
        return namespaces.Length < 2 ? $"{id}" : $"{namespaces[^2]}.{id}";
    }
}