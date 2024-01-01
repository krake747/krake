namespace Krake.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Portfolios
    {
        private const string Base = $"{ApiBase}/portfolios";

        public const string Get = $"{Base}/{{id:Guid}}";
        public const string GetAll = Base;
    }
}