namespace Krake.Infrastructure.Email.Models.Options;

public sealed class OutgoingEmailServerOptions
{
    public MailServerType Type { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public bool UseStartTls { get; set; }
}