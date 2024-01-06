using System.Reflection;
using System.Text;
using Krake.Infrastructure.IO.Common.Attributes;

namespace Krake.Cli.Features;

public static class FileCreator
{
    public static IEnumerable<string> WriteObjectsToLines<T>(IEnumerable<T> objects, char delimiter)
        where T : notnull
    {
        var data = objects.ToArray();
        return data.Length is not 0
            ? WriteObjectsToLines(data, delimiter)
            : Enumerable.Empty<string>();
    }

    public static IEnumerable<string> WriteObjectsToLines<T1, T2>(IEnumerable<T1> objectsOne,
        IEnumerable<T2> objectsTwo, char delimiter)
        where T1 : notnull
        where T2 : notnull
    {
        var dataOne = objectsOne.ToArray();
        var dataTwo = objectsTwo.ToArray();

        return dataOne.Length is not 0 && dataTwo.Length is not 0 && dataOne.Length == dataTwo.Length
            ? WriteObjectsToLines(dataOne, dataTwo, delimiter)
            : Enumerable.Empty<string>();
    }

    private static IEnumerable<string> WriteObjectsToLines<T>(IReadOnlyList<T> objects, char delimiter)
        where T : notnull
    {
        var headers = CreateHeaderLines(objects, delimiter);
        var lines = CreateDataLines(objects, delimiter);
        return headers.Concat(lines);
    }

    private static IEnumerable<string> WriteObjectsToLines<T1, T2>(IReadOnlyList<T1> objectsOne,
        IReadOnlyList<T2> objectsTwo, char delimiter = '\t')
        where T1 : notnull
        where T2 : notnull
    {
        var headerOne = CreateHeaderLines(objectsOne, delimiter);
        var headerTwo = CreateHeaderLines(objectsTwo, delimiter);

        var headers = new[]
        {
            headerOne.Concat(headerTwo)
                .Aggregate(new StringBuilder(), (seed, acc) => seed.Append($"{delimiter}{acc}"))
                .Remove(0, 1)
                .ToString()
        };

        var valuesOne = CreateDataLines(objectsOne, delimiter);
        var valuesTwo = CreateDataLines(objectsTwo, delimiter);

        var values = valuesOne.Zip(valuesTwo, (lhs, rhs) => $"{lhs}{delimiter}{rhs}");

        return headers.Concat(values);
    }

    private static IEnumerable<string> CreateDataLines<T>(IEnumerable<T> objects, char delimiter)
        where T : notnull =>
        objects.Select(obj => new SomeObj<T>(obj, obj.GetType().GetProperties()))
            .Select(some => CreateDataLine(some, delimiter));

    private static string CreateDataLine<T>(SomeObj<T> some, char delimiter)
        where T : notnull =>
        some.Properties
            .Select(p =>
            {
                var value = p.GetValue(some.Object);
                return value switch
                {
                    DateOnly dateOnly => $"{dateOnly:O}",
                    _ => value?.ToString() ?? ""
                };
            })
            .Aggregate(new StringBuilder(), (seed, acc) => seed.Append($"{delimiter}{acc}"))
            .Remove(0, 1)
            .ToString();

    private static IEnumerable<string> CreateHeaderLines<T>(IReadOnlyList<T> objects, char delimiter)
        where T : notnull
    {
        yield return CreateHeaderLine(objects, delimiter);
    }

    private static string CreateHeaderLine<T>(IReadOnlyList<T> objects, char delimiter)
        where T : notnull =>
        objects[0].GetType()
            .GetProperties()
            .Select(p =>
            {
                var attribute = p.GetCustomAttributes(typeof(ColumnNameAttribute), true)
                    .Cast<ColumnNameAttribute>()
                    .FirstOrDefault();

                var headerName = attribute is null ? p.Name : attribute.Name;
                return headerName;
            })
            .Aggregate(new StringBuilder(), (seed, acc) => seed.Append($"{delimiter}{acc}"))
            .Remove(0, 1)
            .ToString();

    private readonly record struct SomeObj<T>(T Object, PropertyInfo[] Properties)
        where T : notnull;
}