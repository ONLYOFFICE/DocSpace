/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using Message = Amazon.SimpleEmail.Model.Message;

namespace ASC.Core.Notify.Senders;

[Singletone(Additional = typeof(AWSSenderExtension))]
public class AWSSender : SmtpSender
{
    private readonly object _locker = new object();
    private AmazonSimpleEmailServiceClient _amazonEmailServiceClient;
    private TimeSpan _refreshTimeout;
    private DateTime _lastRefresh;
    private DateTime _lastSend;
    private TimeSpan _sendWindow = TimeSpan.MinValue;
    private GetSendQuotaResponse _quota;

    public AWSSender(IServiceProvider serviceProvider,
        IOptionsMonitor<ILog> options) : base(serviceProvider, options)
    {
        Logger = options.Get("ASC.Notify.AmazonSES");
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
                Logger.DebugFormat("Tenant: {0}, To: {1}", m.TenantId, m.Reciever);
                using var scope = ServiceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<AWSSenderScope>();
                var (tenantManager, configuration) = scopeClass;
                tenantManager.SetCurrentTenant(m.TenantId);

                if (!configuration.SmtpSettings.IsDefaultSettings)
                {
                    UseCoreSettings = true;
                    result = base.Send(m);
                    UseCoreSettings = false;
                }
                else
                {
                    result = SendMessage(m);
                }

                Logger.DebugFormat(result.ToString());
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Tenant: {0}, To: {1} - {2}", m.TenantId, m.Reciever, e);
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
            Logger.DebugFormat("Amazon sending failed: {0}, fallback to smtp", result);
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
                    Logger.WarnFormat("Quota limit reached. setting next check to: {0}", _lastRefresh);

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
                Logger.DebugFormat("Send rate doesn't fit in send window. sleeping for: {0}", _sendWindow);
                Thread.Sleep(_sendWindow);
            }
        }
    }

    private void RefreshQuotaIfNeeded()
    {
        if (!IsRefreshNeeded()) return;

        lock (_locker)
        {
            if (IsRefreshNeeded())//Double check
            {
                Logger.DebugFormat("refreshing qouta. interval: {0} Last refresh was at: {1}", _refreshTimeout, _lastRefresh);

                //Do quota refresh
                _lastRefresh = DateTime.UtcNow.AddMinutes(1);
                try
                {
                    var r = new GetSendQuotaRequest();
                    _quota = _amazonEmailServiceClient.GetSendQuotaAsync(r).Result;
                    _sendWindow = TimeSpan.FromSeconds(1.0 / _quota.MaxSendRate);
                    Logger.DebugFormat("quota: {0}/{1} at {2} mps. send window:{3}", _quota.SentLast24Hours, _quota.Max24HourSend, _quota.MaxSendRate, _sendWindow);
                }
                catch (Exception e)
                {
                    Logger.Error("error refreshing quota", e);
                }
            }
        }
    }

    private bool IsRefreshNeeded()
    {
        return _quota == null || (DateTime.UtcNow - _lastRefresh) > _refreshTimeout;
    }
}

[Scope]
public class AWSSenderScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreConfiguration _coreConfiguration;

    public AWSSenderScope(TenantManager tenantManager, CoreConfiguration coreConfiguration)
    {
        _tenantManager = tenantManager;
        _coreConfiguration = coreConfiguration;
    }

    public void Deconstruct(out TenantManager tenantManager, out CoreConfiguration coreConfiguration)
    {
        (tenantManager, coreConfiguration) = (_tenantManager, _coreConfiguration);
    }
}

public static class AWSSenderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<AWSSenderScope>();
    }
}
