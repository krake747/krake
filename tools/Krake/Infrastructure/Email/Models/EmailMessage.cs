namespace Krake.Infrastructure.Email.Models;

public sealed class EmailMessage
{
    public List<string> To { get; } = [];
    public List<string> Cc { get; } = [];
    public List<string> Bcc { get; } = [];

    public string? From { get; set; }
    public string? DisplayName { get; set; }
    public string? ReplyTo { get; set; }
    public string? ReplyToName { get; set; }

    public string? Subject { get; set; }
    public string? Body { get; set; }
    public EmailBodyFormat BodyFormat { get; set; }
}