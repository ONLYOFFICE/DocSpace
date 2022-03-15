namespace ASC.Core.Notify.Senders;

[Singletone(Additional = typeof(SmtpSenderExtension))]
public class SmtpSender : INotifySender
{
    protected ILog Logger { get; set; }
    protected readonly IConfiguration Configuration;
    protected IServiceProvider ServiceProvider;

    private string _host;
    private int _port;
    private bool _ssl;
    private ICredentials _credentials;
    protected bool UseCoreSettings;
    const int NetworkTimeout = 30000;

    public SmtpSender(
        IServiceProvider serviceProvider,
        IOptionsMonitor<ILog> options)
    {
        Logger = options.Get("ASC.Notify");
        Configuration = serviceProvider.GetService<IConfiguration>();
        ServiceProvider = serviceProvider;
    }

    public virtual void Init(IDictionary<string, string> properties)
    {
        if (properties.ContainsKey("useCoreSettings") && bool.Parse(properties["useCoreSettings"]))
        {
            UseCoreSettings = true;
        }
        else
        {
            _host = properties["host"];
            _port = properties.ContainsKey("port") ? int.Parse(properties["port"]) : 25;
            _ssl = properties.ContainsKey("enableSsl") && bool.Parse(properties["enableSsl"]);
            if (properties.TryGetValue("userName", out var property))
            {
                _credentials = new NetworkCredential(property, properties["password"]);
            }
        }
    }

    private void InitUseCoreSettings(CoreConfiguration configuration)
    {
        var s = configuration.SmtpSettings;

        _host = s.Host;
        _port = s.Port;
        _ssl = s.EnableSSL;
        _credentials = !string.IsNullOrEmpty(s.CredentialsUserName)
            ? new NetworkCredential(s.CredentialsUserName, s.CredentialsUserPassword)
            : null;
    }

    public virtual NoticeSendResult Send(NotifyMessage m)
    {
        using var scope = ServiceProvider.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<SmtpSenderScope>();
        var (tenantManager, configuration) = scopeClass;
        tenantManager.SetCurrentTenant(m.TenantId);

        var smtpClient = GetSmtpClient();
        var result = NoticeSendResult.TryOnceAgain;
        try
        {
            try
            {
                if (UseCoreSettings)
                {
                    InitUseCoreSettings(configuration);
                }

                var mail = BuildMailMessage(m);

                Logger.DebugFormat("SmtpSender - host={0}; port={1}; enableSsl={2} enableAuth={3}", _host, _port, _ssl, _credentials != null);

                smtpClient.Connect(_host, _port,
                    _ssl ? SecureSocketOptions.Auto : SecureSocketOptions.None);

                if (_credentials != null)
                {
                    smtpClient.Authenticate(_credentials);
                }

                smtpClient.Send(mail);
                result = NoticeSendResult.OK;
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Tenant: {0}, To: {1} - {2}", m.TenantId, m.Reciever, e);

                throw;
            }
        }
        catch (ObjectDisposedException)
        {
            result = NoticeSendResult.SendingImpossible;
        }
        catch (InvalidOperationException)
        {
            result = string.IsNullOrEmpty(_host) || _port == 0
                ? NoticeSendResult.SendingImpossible
                : NoticeSendResult.TryOnceAgain;
        }
        catch (IOException)
        {
            result = NoticeSendResult.TryOnceAgain;
        }
        catch (MailKit.Net.Smtp.SmtpProtocolException)
        {
            result = NoticeSendResult.SendingImpossible;
        }
        catch (MailKit.Net.Smtp.SmtpCommandException e)
        {
            switch (e.StatusCode)
            {
                case MailKit.Net.Smtp.SmtpStatusCode.MailboxBusy:
                case MailKit.Net.Smtp.SmtpStatusCode.MailboxUnavailable:
                case MailKit.Net.Smtp.SmtpStatusCode.ExceededStorageAllocation:
                    result = NoticeSendResult.TryOnceAgain;
                    break;
                case MailKit.Net.Smtp.SmtpStatusCode.MailboxNameNotAllowed:
                case MailKit.Net.Smtp.SmtpStatusCode.UserNotLocalWillForward:
                case MailKit.Net.Smtp.SmtpStatusCode.UserNotLocalTryAlternatePath:
                    result = NoticeSendResult.MessageIncorrect;
                    break;
                default:
                    if (e.StatusCode != MailKit.Net.Smtp.SmtpStatusCode.Ok)
                    {
                        result = NoticeSendResult.TryOnceAgain;
                    }
                    break;
            }
        }
        catch (Exception)
        {
            result = NoticeSendResult.SendingImpossible;
        }
        finally
        {
            if (smtpClient.IsConnected)
            {
                smtpClient.Disconnect(true);
            }

            smtpClient.Dispose();
        }

        return result;
    }

    private MimeMessage BuildMailMessage(NotifyMessage m)
    {
        var mimeMessage = new MimeMessage
        {
            Subject = m.Subject
        };

        var fromAddress = MailboxAddress.Parse(ParserOptions.Default, m.Sender);

        mimeMessage.From.Add(fromAddress);

        foreach (var to in m.Reciever.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
        {
            mimeMessage.To.Add(MailboxAddress.Parse(ParserOptions.Default, to));
        }

        if (m.ContentType == Pattern.HtmlContentType)
        {
            var textPart = new TextPart("plain")
            {
                Text = HtmlUtil.GetText(m.Content),
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };

            var multipartAlternative = new MultipartAlternative { textPart };

            var htmlPart = new TextPart("html")
            {
                Text = GetHtmlView(m.Content),
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };

            if (m.Attachments != null && m.Attachments.Count > 0)
            {
                var multipartRelated = new MultipartRelated
                {
                    Root = htmlPart
                };

                foreach (var attachment in m.Attachments)
                {
                    var mimeEntity = ConvertAttachmentToMimePart(attachment);
                    if (mimeEntity != null)
                    {
                        multipartRelated.Add(mimeEntity);
                    }
                }

                multipartAlternative.Add(multipartRelated);
            }
            else
            {
                multipartAlternative.Add(htmlPart);
            }

            mimeMessage.Body = multipartAlternative;
        }
        else
        {
            mimeMessage.Body = new TextPart("plain")
            {
                Text = m.Content,
                ContentTransferEncoding = ContentEncoding.QuotedPrintable
            };
        }

        if (!string.IsNullOrEmpty(m.ReplyTo))
        {
            mimeMessage.ReplyTo.Add(MailboxAddress.Parse(ParserOptions.Default, m.ReplyTo));
        }

        mimeMessage.Headers.Add("Auto-Submitted", string.IsNullOrEmpty(m.AutoSubmitted) ? "auto-generated" : m.AutoSubmitted);

        return mimeMessage;
    }

    protected string GetHtmlView(string body)
    {
        return $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
                      <html>
                        <head>
                            <meta content=""text/html;charset=UTF-8"" http-equiv=""Content-Type"">
                        </head>
                        <body>{body}</body>
                      </html>";
    }

    private MailKit.Net.Smtp.SmtpClient GetSmtpClient()
    {
        var smtpClient = new MailKit.Net.Smtp.SmtpClient
        {
            Timeout = NetworkTimeout
        };

        return smtpClient;
    }

    private static MimePart ConvertAttachmentToMimePart(NotifyMessageAttachment attachment)
    {
        try
        {
            if (attachment == null || string.IsNullOrEmpty(attachment.FileName) || string.IsNullOrEmpty(attachment.ContentId) || attachment.Content == null)
            {
                return null;
            }

            var extension = Path.GetExtension(attachment.FileName);

            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }

            return new MimePart("image", extension.TrimStart('.'))
            {
                ContentId = attachment.ContentId,
                Content = new MimeContent(new MemoryStream(attachment.Content.ToByteArray())),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = attachment.FileName
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}

[Scope]
public class SmtpSenderScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreConfiguration _coreConfiguration;

    public SmtpSenderScope(TenantManager tenantManager, CoreConfiguration coreConfiguration)
    {
        _tenantManager = tenantManager;
        _coreConfiguration = coreConfiguration;
    }

    public void Deconstruct(out TenantManager tenantManager, out CoreConfiguration coreConfiguration)
    {
        (tenantManager, coreConfiguration) = (_tenantManager, _coreConfiguration);
    }
}

public static class SmtpSenderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SmtpSenderScope>();
    }
}
