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
public class AWSSender : SmtpSender, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
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

    public override async Task<NoticeSendResult> SendAsync(NotifyMessage m)
    {
        NoticeSendResult result;
        try
        {
            try
            {
                _logger.DebugSendTo(m.TenantId, m.Reciever);
                await using var scope = _serviceProvider.CreateAsyncScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                await tenantManager.SetCurrentTenantAsync(m.TenantId);

                var configuration = scope.ServiceProvider.GetService<CoreConfiguration>();
                if (!(await configuration.GetDefaultSmtpSettingsAsync()).IsDefaultSettings)
                {
                    result = await base.SendAsync(m);
                }
                else
                {
                    result = await SendMessage(m);
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
            result = await base.SendAsync(m);
        }

        return result;
    }

    private async Task<NoticeSendResult> SendMessage(NotifyMessage m)
    {
        //Check if we need to query stats
        await RefreshQuotaIfNeeded();
        if (_quota != null)
        {
            await _semaphore.WaitAsync();
            if (_quota.Max24HourSend <= _quota.SentLast24Hours)
            {
                //Quota exceeded, queue next refresh to +24 hours
                _lastRefresh = DateTime.UtcNow.AddHours(24);
                _logger.WarningQuotaLimit(_lastRefresh);

                return NoticeSendResult.SendingImpossible;
            }
            _semaphore.Release();
        }

        var message = BuildMailMessage(m);

        using var ms = new MemoryStream();
        message.WriteTo(ms);

        var request = new SendRawEmailRequest(new RawMessage(ms));

        ThrottleIfNeeded();

        var response = await _amazonEmailServiceClient.SendRawEmailAsync(request);

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

    private async Task RefreshQuotaIfNeeded()
    {
        if (!IsRefreshNeeded())
        {
            return;
        }

        await _semaphore.WaitAsync();
        if (IsRefreshNeeded())//Double check
        {
            _logger.DebugRefreshingQuota(_refreshTimeout, _lastRefresh);

            //Do quota refresh
            _lastRefresh = DateTime.UtcNow.AddMinutes(1);
            try
            {
                var r = new GetSendQuotaRequest();
                _quota = await _amazonEmailServiceClient.GetSendQuotaAsync(r);
                _sendWindow = TimeSpan.FromSeconds(1.0 / _quota.MaxSendRate);
                _logger.DebugQuota(_quota.SentLast24Hours, _quota.Max24HourSend, _quota.MaxSendRate, _sendWindow);
            }
            catch (Exception e)
            {
                _logger.ErrorRefreshingQuota(e);
            }
        }
        _semaphore.Release();
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
