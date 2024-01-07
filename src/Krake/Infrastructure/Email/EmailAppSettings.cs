using Krake.Infrastructure.Email.Models.Options;

namespace Krake.Infrastructure.Email;

public sealed class EmailAppSettings
{
    public const string SectionName = "Email";
    public EmailAuthOptions? EmailAuth { get; set; }
    public DefaultSenderOptions? DefaultSender { get; set; }
    public IncomingEmailServerOptions? IncomingEmailServer { get; set; }
    public OutgoingEmailServerOptions? OutgoingEmailServer { get; set; }
}