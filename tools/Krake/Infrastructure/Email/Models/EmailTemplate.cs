namespace Krake.Infrastructure.Email.Models;

public sealed class EmailTemplate
{
    public string? DisplayName { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public EmailBodyFormat BodyFormat { get; set; }
}