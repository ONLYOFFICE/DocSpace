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

using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Webhooks;
using ASC.Webhooks.Core;
using ASC.Webhooks.Core.EF.Model;
using ASC.Webhooks.Service;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using Moq;

using NUnit.Framework;

namespace ASC.Webhooks.Tests
{
    [TestFixture]
    public class CommonWebhooksTests : BaseSetUp
    {
        private readonly string _eventName = "testEvent";
        private readonly string _secretKey = "testSecretKey";
        private readonly string _content = "testContent";
        private readonly string _contentSerialize = JsonSerializer.Serialize("testContent");
        private readonly string _uri = $"http://localhost:{_port}/api/2.0/Test/";
        private readonly DateTime _creationTime = DateTime.Now;
        private readonly CacheNotifyAction _testCacheNotifyAction = CacheNotifyAction.Update;

        [Order(1)]
        [Test]
        public async Task Publisher()
        {
            var scope = _host.Services.CreateScope();
            var dbWorker = scope.ServiceProvider.GetService<DbWorker>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

            var id = 1;
            var testWebhookRequest = new WebhookRequest { Id = id };
            var testTenant = new Tenant(1, "testWebhooksPublisher");
            try
            {
                tenantManager.SetCurrentTenant(testTenant);

                await dbWorker.AddWebhookConfig(_eventName, _uri, _secretKey);

                var mockedKafkaCaches = new Mock<ICacheNotify<WebhookRequest>>();
                mockedKafkaCaches.Setup(a => a.Publish(testWebhookRequest, _testCacheNotifyAction)).Verifiable();

                var publisher = new WebhookPublisher(dbWorker, mockedKafkaCaches.Object);
                await publisher.PublishAsync(_eventName, "", _contentSerialize);

                mockedKafkaCaches.Verify(a => a.Publish(testWebhookRequest, _testCacheNotifyAction), Times.Once);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.AreEqual((await dbWorker.ReadJournal(id)).RequestPayload, _contentSerialize);
        }
        
        [Order(2)]
        [Test]
        public async Task Sender()
        {
            var scope = _host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var dbWorker = serviceProvider.GetService<DbWorker>();
            var tenantManager = serviceProvider.GetService<TenantManager>();

            var testTenant = new Tenant(2, "testWebhooksSender");

            tenantManager.SetCurrentTenant(testTenant);


            var successedId = (await dbWorker.AddWebhookConfig(_eventName, $"{_uri}SuccessRequest/", _secretKey)).Id;
            var failedId = (await dbWorker.AddWebhookConfig(_eventName, $"{_uri}FailedRequest/", _secretKey)).Id;

            var successWebhookPayload = new WebhooksLog { ConfigId = successedId, Status = 200, CreationTime = _creationTime, RequestPayload = _contentSerialize };
            var failedWebhookPayload = new WebhooksLog { ConfigId = failedId, Status = 400, CreationTime = _creationTime, RequestPayload = _contentSerialize };
            var successWebhookPayloadId = (await dbWorker.WriteToJournal(successWebhookPayload)).Id;
            var failedWebhookPayloadId = (await dbWorker.WriteToJournal(failedWebhookPayload)).Id;

            var source = new CancellationTokenSource();
            var token = source.Token;

            var SuccessedWebhookRequest = new WebhookRequest { Id = successWebhookPayloadId };
            var FailedWebhookRequest = new WebhookRequest { Id = failedWebhookPayloadId };

            var sender = new WebhookSender(serviceProvider.GetService<ILoggerProvider>(), serviceProvider.GetRequiredService<IServiceScopeFactory>(), _httpClientFactory);
            await sender.Send(SuccessedWebhookRequest, token);
            await sender.Send(FailedWebhookRequest, token);

            Assert.IsTrue(_requestHistory.SuccessCounter == 1, "Problem with successed request");
            Assert.IsTrue(_requestHistory.FailedCounter == 1, "Problem with failed request");
            Assert.IsTrue(_requestHistory.СorrectSignature, "Problem with signature");
        }
        
        [Test]
        public async Task GlobalFilter()
        {
            try
            {
                var controllerAddress = "api/2.0/Test/testMethod";

                var mockedWebhookPubslisher = new Mock<IWebhookPublisher>();
                mockedWebhookPubslisher.Setup(a => a.PublishAsync("GET", controllerAddress, _content)).Verifiable();
                mockedWebhookPubslisher.Setup(a => a.PublishAsync("POST", controllerAddress, _content)).Verifiable();


                using var host = await new HostBuilder()
                    .ConfigureWebHost(webBuilder =>
                    {
                        webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(mockedWebhookPubslisher.Object);
                            services.AddControllers();

                            services.AddSingleton(new Action<JsonOptions>(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never));

                            var dIHelper = new DIHelper();
                            dIHelper.Configure(services);
                            dIHelper.TryAdd<TestController>();
                            dIHelper.TryAdd<WebhooksGlobalFilterAttribute>();

                            var builder = services.AddMvcCore(config =>
                            {
                                config.Filters.Add(new TypeFilterAttribute(typeof(WebhooksGlobalFilterAttribute)));
                            });
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapCustom();
                            });
                        });
                    })
                    .StartAsync();

                var getResponse = await host.GetTestClient().GetAsync(controllerAddress);
                mockedWebhookPubslisher.Verify(a => a.PublishAsync("GET", controllerAddress, _content), Times.Never);

                var stringContent = new StringContent(_content);

                var postResponse = await host.GetTestClient().PostAsync(controllerAddress, stringContent);
                mockedWebhookPubslisher.Verify(a => a.PublishAsync("POST", controllerAddress, _content), Times.Once);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.Pass();
        }
    }
}