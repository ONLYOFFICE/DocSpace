// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Notify.Senders;

[Singletone]
public class SmtpSender : INotifySender
{
    protected ILogger _logger;
    private IDictionary<string, string> _initProperties;
    protected readonly IConfiguration _configuration;
    protected IServiceProvider _serviceProvider;

    private string _host;
    private int _port;
    private bool _ssl;
    private ICredentials _credentials;
    protected bool _useCoreSettings;
    const int NetworkTimeout = 30000;

    public SmtpSender(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILoggerProvider options)
    {
        _initProperties = new Dictionary<string, string>();
        _logger = options.CreateLogger("ASC.Notify");
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    public virtual void Init(IDictionary<string, string> properties)
    {
        _initProperties = properties;
    }

    public virtual async Task<NoticeSendResult> SendAsync(NotifyMessage m)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        await tenantManager.SetCurrentTenantAsync(m.TenantId);

        var configuration = scope.ServiceProvider.GetService<CoreConfiguration>();

        var smtpClient = GetSmtpClient();
        var result = NoticeSendResult.TryOnceAgain;
        try
        {
            try
            {
                await BuildSmtpSettingsAsync(configuration);

                var mail = BuildMailMessage(m);

                _logger.DebugSmtpSender(_host, _port, _ssl, _credentials != null);

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
                _logger.ErrorSend(m.TenantId, m.Reciever, e);

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

    private async Task BuildSmtpSettingsAsync(CoreConfiguration configuration)
    {
        if ((await configuration.GetSmtpSettingsAsync()).IsDefaultSettings && _initProperties.ContainsKey("host") && !string.IsNullOrEmpty(_initProperties["host"]))
        {
            _host = _initProperties["host"];

            if (_initProperties.ContainsKey("port") && !string.IsNullOrEmpty(_initProperties["port"]))
            {
                _port = int.Parse(_initProperties["port"]);
            }
            else
            {
                _port = 25;
            }

            if (_initProperties.ContainsKey("enableSsl") && !string.IsNullOrEmpty(_initProperties["enableSsl"]))
            {
                _ssl = bool.Parse(_initProperties["enableSsl"]);
            }
            else
            {
                _ssl = false;
            }

            if (_initProperties.ContainsKey("userName"))
            {
                _credentials = new NetworkCredential(
                     _initProperties["userName"],
                     _initProperties["password"]);
            }
        }
        else
        {
            var s = await configuration.GetSmtpSettingsAsync();

            _host = s.Host;
            _port = s.Port;
            _ssl = s.EnableSSL;
            _credentials = !string.IsNullOrEmpty(s.CredentialsUserName)
                ? new NetworkCredential(s.CredentialsUserName, s.CredentialsUserPassword)
                : null;
        }
    }
    protected MimeMessage BuildMailMessage(NotifyMessage m)
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

            if (m.Attachments != null && m.Attachments.Length > 0)
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
                Content = new MimeContent(new MemoryStream(attachment.Content)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
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
