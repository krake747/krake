using Krake.Infrastructure.Email.Models;

namespace Krake.Infrastructure.Email.Builders;

public interface IMandatoryEmailBuilder<out TEmailBuilder>
{
    TEmailBuilder From(string from);
    TEmailBuilder To(string to);
    TEmailBuilder Subject(string subject);
    TEmailBuilder Body(string body, EmailBodyFormat bodyFormat);
}