namespace Krake.Modules.Portfolios.Presentation;

internal static class OpenApiSchemas
{
    internal static class Accepts
    {
        private const string Base = "application";
        public const string Json = $"{Base}/json";
    }

    internal static class Tags
    {
        public const string Portfolios = nameof(Portfolios);
    }
}