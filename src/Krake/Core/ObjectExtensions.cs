namespace Krake.Core;

public static class ObjectExtensions
{
    public static string ToValueOrDefault(this object obj, string fallback = "") =>
        obj.ToString() ?? fallback;

    public static int ToValueOrDefault(this object obj, int fallback) =>
        int.TryParse(obj.ToString(), out var value) ? value : fallback;

    public static int ToValueOrDefault(this object obj, int fallback, IFormatProvider? formatProvider) =>
        int.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static double ToValueOrDefault(this object obj, double fallback) =>
        double.TryParse(obj.ToString(), out var value) ? value : fallback;

    public static double ToValueOrDefault(this object obj, double fallback, IFormatProvider? formatProvider) =>
        double.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static float ToValueOrDefault(this object obj, float fallback) =>
        float.TryParse(obj.ToString(), out var value) ? value : fallback;

    public static float ToValueOrDefault(this object obj, float fallback, IFormatProvider? formatProvider) =>
        float.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;

    public static decimal ToValueOrDefault(this object obj, decimal fallback) =>
        decimal.TryParse(obj.ToString(), out var value) ? value : fallback;

    public static decimal ToValueOrDefault(this object obj, decimal fallback, IFormatProvider? formatProvider) =>
        decimal.TryParse(obj.ToString(), formatProvider, out var value) ? value : fallback;
}