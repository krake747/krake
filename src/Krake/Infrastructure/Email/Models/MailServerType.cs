using System.Text.Json.Serialization;

namespace Krake.Infrastructure.Email.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MailServerType
{
    Smtp,
    Imap,
    Pop3
}