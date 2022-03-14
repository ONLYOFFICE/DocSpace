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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Core.Notify.Senders
{
    [Singletone(Additional = typeof(AWSSenderExtension))]
    public class AWSSender : SmtpSender
    {
        private readonly object locker = new object();
        private AmazonSimpleEmailServiceClient ses;
        private TimeSpan refreshTimeout;
        private DateTime lastRefresh;
        private DateTime lastSend;
        private TimeSpan sendWindow = TimeSpan.MinValue;
        private GetSendQuotaResponse quota;

        public AWSSender(IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> options) : base(serviceProvider, options)
        {
            Log = options.Get("ASC.Notify.AmazonSES");
        }

        public override void Init(IDictionary<string, string> properties)
        {
            base.Init(properties);
            var region = properties.ContainsKey("region") ? RegionEndpoint.GetBySystemName(properties["region"]) : RegionEndpoint.USEast1;
            ses = new AmazonSimpleEmailServiceClient(properties["accessKey"], properties["secretKey"], region);
            refreshTimeout = TimeSpan.Parse(properties.ContainsKey("refreshTimeout") ? properties["refreshTimeout"] : "0:30:0");
            lastRefresh = DateTime.UtcNow - refreshTimeout; //set to refresh on first send
        }

        public override NoticeSendResult Send(NotifyMessage m)
        {
            NoticeSendResult result;
            try
            {
                try
                {
                    Log.DebugFormat("Tenant: {0}, To: {1}", m.Tenant, m.To);
                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<AWSSenderScope>();
                    var (tenantManager, configuration) = scopeClass;
                    tenantManager.SetCurrentTenant(m.Tenant);

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

                    Log.DebugFormat(result.ToString());
                }
                catch (Exception e)
                {
                    Log.ErrorFormat("Tenant: {0}, To: {1} - {2}", m.Tenant, m.To, e);
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
                Log.DebugFormat("Amazon sending failed: {0}, fallback to smtp", result);
                result = base.Send(m);
            }
            return result;
        }

        private NoticeSendResult SendMessage(NotifyMessage m)
        {
            //Check if we need to query stats
            RefreshQuotaIfNeeded();
            if (quota != null)
            {
                lock (locker)
                {
                    if (quota.Max24HourSend <= quota.SentLast24Hours)
                    {
                        //Quota exceeded, queue next refresh to +24 hours
                        lastRefresh = DateTime.UtcNow.AddHours(24);
                        Log.WarnFormat("Quota limit reached. setting next check to: {0}", lastRefresh);
                        return NoticeSendResult.SendingImpossible;
                    }
                }
            }

            var dest = new Destination
            {
                ToAddresses = m.To.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(a => MailAddressUtils.Create(a).Address).ToList(),
            };

            var subject = new Content(MimeHeaderUtils.EncodeMime(m.Subject)) { Charset = Encoding.UTF8.WebName, };

            Body body;
            if (m.ContentType == Pattern.HTMLContentType)
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

            var from = MailAddressUtils.Create(m.From).ToEncodedString();
            var request = new SendEmailRequest { Source = from, Destination = dest, Message = new Message(subject, body) };
            if (!string.IsNullOrEmpty(m.ReplyTo))
            {
                request.ReplyToAddresses.Add(MailAddressUtils.Create(m.ReplyTo).Address);
            }

            ThrottleIfNeeded();

            var response = ses.SendEmailAsync(request).Result;
            lastSend = DateTime.UtcNow;

            return response != null ? NoticeSendResult.OK : NoticeSendResult.TryOnceAgain;
        }


        private void ThrottleIfNeeded()
        {
            //Check last send and throttle if needed
            if (sendWindow != TimeSpan.MinValue)
            {
                if (DateTime.UtcNow - lastSend <= sendWindow)
                {
                    //Possible BUG: at high frequncies maybe bug with to little differences
                    //This means that time passed from last send is less then message per second
                    Log.DebugFormat("Send rate doesn't fit in send window. sleeping for: {0}", sendWindow);
                    Thread.Sleep(sendWindow);
                }
            }
        }

        private void RefreshQuotaIfNeeded()
        {
            if (!IsRefreshNeeded()) return;

            lock (locker)
            {
                if (IsRefreshNeeded())//Double check
                {
                    Log.DebugFormat("refreshing qouta. interval: {0} Last refresh was at: {1}", refreshTimeout, lastRefresh);

                    //Do quota refresh
                    lastRefresh = DateTime.UtcNow.AddMinutes(1);
                    try
                    {
                        var r = new GetSendQuotaRequest();
                        quota = ses.GetSendQuotaAsync(r).Result;
                        sendWindow = TimeSpan.FromSeconds(1.0 / quota.MaxSendRate);
                        Log.DebugFormat("quota: {0}/{1} at {2} mps. send window:{3}", quota.SentLast24Hours, quota.Max24HourSend, quota.MaxSendRate, sendWindow);
                    }
                    catch (Exception e)
                    {
                        Log.Error("error refreshing quota", e);
                    }
                }
            }
        }

        private bool IsRefreshNeeded()
        {
            return quota == null || (DateTime.UtcNow - lastRefresh) > refreshTimeout;
        }
    }

    [Scope]
    public class AWSSenderScope
    {
        private TenantManager TenantManager { get; }
        private CoreConfiguration CoreConfiguration { get; }

        public AWSSenderScope(TenantManager tenantManager, CoreConfiguration coreConfiguration)
        {
            TenantManager = tenantManager;
            CoreConfiguration = coreConfiguration;
        }

        public void Deconstruct(out TenantManager tenantManager, out CoreConfiguration coreConfiguration)
        {
            (tenantManager, coreConfiguration) = (TenantManager, CoreConfiguration);
        }
    }

    public static class AWSSenderExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<AWSSenderScope>();
        }
    }
}
