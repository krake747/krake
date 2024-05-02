namespace Krake.Core.Extensions;

public static class ObjectExtensions
{
    public static string ToValueOrDefault(this object obj, string fallback = "") =>
        obj.ToString() ?? fallback;

    public static int ToValueOrDefault(this object obj, int fallback, IFormatProvider? formatProvider = null) =>
        int.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static double ToValueOrDefault(this object obj, double fallback, IFormatProvider? formatProvider = null) =>
        double.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static float ToValueOrDefault(this object obj, float fallback, IFormatProvider? formatProvider = null) =>
        float.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static decimal ToValueOrDefault(this object obj, decimal fallback, IFormatProvider? formatProvider = null) =>
        decimal.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;
}