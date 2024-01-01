namespace Krake.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Portfolios
    {
        private const string Base = $"{ApiBase}/portfolios";

        public const string Create = Base;
        public const string Get = $"{Base}/{{id:Guid}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:Guid}}";
        public const string Delete = $"{Base}/{{id:Guid}}";
    }
}