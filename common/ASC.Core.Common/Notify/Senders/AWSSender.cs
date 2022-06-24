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

using Message = Amazon.SimpleEmail.Model.Message;

namespace ASC.Core.Notify.Senders;

[Singletone]
public class AWSSender : SmtpSender, IDisposable
{
    private readonly object _locker = new object();
    private AmazonSimpleEmailServiceClient _amazonEmailServiceClient;
    private TimeSpan _refreshTimeout;
    private DateTime _lastRefresh;
    private DateTime _lastSend;
    private TimeSpan _sendWindow = TimeSpan.MinValue;
    private GetSendQuotaResponse _quota;

    public AWSSender(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILoggerProvider options) : base(configuration, serviceProvider, options)
    {
        _logger = options.CreateLogger("ASC.Notify.AmazonSES");
    }

    public override void Init(IDictionary<string, string> properties)
    {
        base.Init(properties);
        var region = properties.ContainsKey("region") ? RegionEndpoint.GetBySystemName(properties["region"]) : RegionEndpoint.USEast1;
        _amazonEmailServiceClient = new AmazonSimpleEmailServiceClient(properties["accessKey"], properties["secretKey"], region);
        _refreshTimeout = TimeSpan.Parse(properties.ContainsKey("refreshTimeout") ? properties["refreshTimeout"] : "0:30:0");
        _lastRefresh = DateTime.UtcNow - _refreshTimeout; //set to refresh on first send
    }

    public override NoticeSendResult Send(NotifyMessage m)
    {
        NoticeSendResult result;
        try
        {
            try
            {
                _logger.DebugSendTo(m.TenantId, m.Reciever);
                using var scope = _serviceProvider.CreateScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                tenantManager.SetCurrentTenant(m.TenantId);

                var configuration = scope.ServiceProvider.GetService<CoreConfiguration>();
                if (!configuration.SmtpSettings.IsDefaultSettings)
                {
                    result = base.Send(m);
                }
                else
                {
                    result = SendMessage(m);
                }

                _logger.Debug(result.ToString());
            }
            catch (Exception e)
            {
                _logger.ErrorSend(m.TenantId, m.Reciever, e);
                throw;
            }
        }
        catch (ArgumentException)
        {
            result = NoticeSendResult.MessageIncorrect;
        }
        catch (MessageRejectedException)
        {
            result = NoticeSendResult.SendingImpossible;
        }
        catch (AmazonSimpleEmailServiceException e)
        {
            result = e.ErrorType == ErrorType.Sender ? NoticeSendResult.MessageIncorrect : NoticeSendResult.TryOnceAgain;
        }
        catch (Exception)
        {
            result = NoticeSendResult.SendingImpossible;
        }

        if (result == NoticeSendResult.MessageIncorrect || result == NoticeSendResult.SendingImpossible)
        {
            _logger.DebugAmazonSendingFailed(result);
            result = base.Send(m);
        }

        return result;
    }

    private NoticeSendResult SendMessage(NotifyMessage m)
    {
        //Check if we need to query stats
        RefreshQuotaIfNeeded();
        if (_quota != null)
        {
            lock (_locker)
            {
                if (_quota.Max24HourSend <= _quota.SentLast24Hours)
                {
                    //Quota exceeded, queue next refresh to +24 hours
                    _lastRefresh = DateTime.UtcNow.AddHours(24);
                    _logger.WarningQuotaLimit(_lastRefresh);

                    return NoticeSendResult.SendingImpossible;
                }
            }
        }

        var dest = new Destination
        {
            ToAddresses = m.Reciever.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(a => MailAddressUtils.Create(a).Address).ToList(),
        };

        var subject = new Content(MimeHeaderUtils.EncodeMime(m.Subject)) { Charset = Encoding.UTF8.WebName, };

        Body body;
        if (m.ContentType == Pattern.HtmlContentType)
        {
            body = new Body(new Content(HtmlUtil.GetText(m.Content)) { Charset = Encoding.UTF8.WebName })
            {
                Html = new Content(GetHtmlView(m.Content)) { Charset = Encoding.UTF8.WebName }
            };
        }
        else
        {
            body = new Body(new Content(m.Content) { Charset = Encoding.UTF8.WebName });
        }

        var from = MailAddressUtils.Create(m.Sender).ToEncodedString();
        var request = new SendEmailRequest { Source = from, Destination = dest, Message = new Message(subject, body) };
        if (!string.IsNullOrEmpty(m.ReplyTo))
        {
            request.ReplyToAddresses.Add(MailAddressUtils.Create(m.ReplyTo).Address);
        }

        ThrottleIfNeeded();

        var response = _amazonEmailServiceClient.SendEmailAsync(request).Result;
        _lastSend = DateTime.UtcNow;

        return response != null ? NoticeSendResult.OK : NoticeSendResult.TryOnceAgain;
    }


    private void ThrottleIfNeeded()
    {
        //Check last send and throttle if needed
        if (_sendWindow != TimeSpan.MinValue)
        {
            if (DateTime.UtcNow - _lastSend <= _sendWindow)
            {
                //Possible BUG: at high frequncies maybe bug with to little differences
                //This means that time passed from last send is less then message per second
                _logger.DebugSendRate(_sendWindow);
                Thread.Sleep(_sendWindow);
            }
        }
    }

    private void RefreshQuotaIfNeeded()
    {
        if (!IsRefreshNeeded())
        {
            return;
        }

        lock (_locker)
        {
            if (IsRefreshNeeded())//Double check
            {
                _logger.DebugRefreshingQuota(_refreshTimeout, _lastRefresh);

                //Do quota refresh
                _lastRefresh = DateTime.UtcNow.AddMinutes(1);
                try
                {
                    var r = new GetSendQuotaRequest();
                    _quota = _amazonEmailServiceClient.GetSendQuotaAsync(r).Result;
                    _sendWindow = TimeSpan.FromSeconds(1.0 / _quota.MaxSendRate);
                    _logger.DebugQuota(_quota.SentLast24Hours, _quota.Max24HourSend, _quota.MaxSendRate, _sendWindow);
                }
                catch (Exception e)
                {
                    _logger.ErrorRefreshingQuota(e);
                }
            }
        }
    }

    private bool IsRefreshNeeded()
    {
        return _quota == null || (DateTime.UtcNow - _lastRefresh) > _refreshTimeout;
    }

    public void Dispose()
    {
        if (_amazonEmailServiceClient != null)
        {
            _amazonEmailServiceClient.Dispose();
        }
    }
}
