using Krake.Core.Builders;
using Krake.Infrastructure.Email.Models;

namespace Krake.Infrastructure.Email.Builders;

public sealed class EmailBuilder : FluentFluentFunctionalBuilder<EmailMessage, EmailBuilder>,
    IMandatoryEmailBuilder<EmailBuilder>
{
    public EmailBuilder From(string from) =>
        Do(x => x.From = from);

    public EmailBuilder To(string to) =>
        Do(x => x.To.Add(to));

    public EmailBuilder Subject(string subject) =>
        Do(x => x.Subject = subject);

    public EmailBuilder Body(string body, EmailBodyFormat bodyFormat) =>
        Do(x =>
        {
            x.Body = body;
            x.BodyFormat = bodyFormat;
        });
}