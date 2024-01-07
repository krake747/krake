using Krake.Core;
using Krake.Core.Builders;
using Krake.Infrastructure.Email.Models;
using MimeKit;
using MimeKit.Text;

namespace Krake.Infrastructure.Email.Builders;

public interface IMimeMessageBuilder :
    IResultFunctionalBuilder<Result<Errors, MimeMessage>, MimeMessage, MimeMessageBuilder>,
    IMandatoryEmailBuilder<MimeMessageBuilder>;

public sealed class MimeMessageBuilder : IMimeMessageBuilder
{
    private readonly List<Func<MimeMessage, MimeMessage>> _actions = [];
    private readonly List<string> _attachments = [];
    private readonly Errors _errors = new([]);

    public Result<Errors, MimeMessage> Build() =>
        _errors.Items.Count > 0 ? _errors : _actions.Aggregate(new MimeMessage(), (x, f) => f(x));

    public MimeMessageBuilder Do(Action<MimeMessage> action)
    {
        _actions.Add(message =>
        {
            action(message);
            return message;
        });
        return this;
    }

    public MimeMessageBuilder From(string from) =>
        Do(x => x.From.Add(MailboxAddress.Parse(from)));

    public MimeMessageBuilder To(string to) =>
        Do(x => x.To.Add(MailboxAddress.Parse(to)));

    public MimeMessageBuilder Subject(string subject) =>
        Do(x => x.Subject = subject);

    public MimeMessageBuilder Body(string body, EmailBodyFormat bodyFormat) =>
        Do(x => { CreateBodyWithAttachments(x, body, bodyFormat, Enumerable.Empty<string>()); });

    public MimeMessageBuilder From(string from, string displayName) =>
        Do(x => x.From.Add(new MailboxAddress(displayName, from)));

    public MimeMessageBuilder To(IEnumerable<string> to) =>
        Do(x => x.To.AddRange(to.Select(MailboxAddress.Parse)));

    public MimeMessageBuilder BodyWithAttachments(string body, EmailBodyFormat bodyFormat, string filePath) =>
        FileExists(filePath)
            .Match(AddToErrorReport, AddToAttachments)
            .Do(x => { CreateBodyWithAttachments(x, body, bodyFormat, _attachments); });

    public MimeMessageBuilder BodyWithAttachments(string body, EmailBodyFormat bodyFormat, FileInfo fileInfo) =>
        BodyWithAttachments(body, bodyFormat, fileInfo.FullName);

    public MimeMessageBuilder BodyWithAttachments(string body, EmailBodyFormat bodyFormat,
        IEnumerable<string> filePaths) =>
        filePaths
            .Select(FileExists)
            .Aggregate(this, (builder, filePathResult) => filePathResult.Match(AddToErrorReport, AddToAttachments))
            .Do(x => { CreateBodyWithAttachments(x, body, bodyFormat, _attachments); });

    public MimeMessageBuilder BodyWithAttachments(string body, EmailBodyFormat bodyFormat,
        IEnumerable<FileInfo> fileInfos) =>
        BodyWithAttachments(body, bodyFormat, fileInfos.Select(x => x.FullName));

    private static void CreateBodyWithAttachments(MimeMessage message, string body, EmailBodyFormat bodyFormat,
        IEnumerable<string> attachments)
    {
        var bodybuilder = bodyFormat switch
        {
            EmailBodyFormat.Html => new BodyBuilder { HtmlBody = body },
            _ => new BodyBuilder { TextBody = body }
        };

        foreach (var attachment in attachments)
        {
            bodybuilder.Attachments.Add(attachment);
        }

        message.Body = bodybuilder.ToMessageBody();
    }

    private MimeMessageBuilder AddToErrorReport(Error error)
    {
        _errors.Add(error);
        return this;
    }

    private MimeMessageBuilder AddToAttachments(string file)
    {
        _attachments.Add(file);
        return this;
    }

    private static Result<Error, string> FileExists(string file) =>
        File.Exists(file) is false ? Error.NotFound().WithAttemptedValue(file) : file;

    private static TextFormat EmailTextFormat(EmailBodyFormat bodyFormat) => bodyFormat switch
    {
        EmailBodyFormat.Html => TextFormat.Html,
        _ => TextFormat.Text
    };
}