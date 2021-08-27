using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Webhooks;
using ASC.Webhooks.Core;
using ASC.Webhooks.Core.Dao.Models;
using ASC.Webhooks.Service;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

namespace ASC.Webhooks.Tests
{
    [TestFixture]
    public class CommonWebhooksTests : BaseSetUp
    {
        private string EventName = "testEvent";
        private string secretKey = "testSecretKey";
        private string Content = JsonSerializer.Serialize("testContent");
        private string URI = $"http://localhost:{port}/api/2.0/Test/";
        private DateTime creationTime = DateTime.Now;
        private CacheNotifyAction testCacheNotifyAction = CacheNotifyAction.Update;

        [Order(1)]
        [Test]
        public void Publisher()
        {
            var scope = host.Services.CreateScope();
            var dbWorker = scope.ServiceProvider.GetService<DbWorker>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

            var id = 1;
            var testWebhookRequest = new WebhookRequest { Id = id };
            var testTenant = new Tenant(1, "testWebhooksPublisher");
            var testWebhookConfig = new WebhooksConfig() 
            { 
                SecretKey = secretKey, 
                TenantId = testTenant.TenantId, 
                Uri = URI 
            };
            var testWebhooksEntry = new WebhookEntry()
            {
                Id = id,
                Data = Content,
                SecretKey = secretKey,
                Uri = URI
            };

            try
            {
                tenantManager.SetCurrentTenant(testTenant);

                dbWorker.AddWebhookConfig(testWebhookConfig);

                var mockedLog = new Mock<IOptionsMonitor<ILog>>();
                mockedLog.Setup(a => a.Get("ASC.Webhooks")).Verifiable();

                var mockedKafkaCaches = new Mock<ICacheNotify<WebhookRequest>>();
                mockedKafkaCaches.Setup(a => a.Publish(testWebhookRequest, testCacheNotifyAction)).Verifiable();

                var publisher = new WebhookPublisher(dbWorker, tenantManager, mockedLog.Object, mockedKafkaCaches.Object);
                publisher.Publish(EventName, Content);

                mockedKafkaCaches.Verify(a => a.Publish(testWebhookRequest, testCacheNotifyAction), Times.Once);            
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.AreEqual(dbWorker.ReadFromJournal(id), testWebhooksEntry);
        }

        [Order(2)]
        [Test]
        public async Task Sender()
        {
            var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var dbWorker = serviceProvider.GetService<DbWorker>();
            var tenantManager = serviceProvider.GetService<TenantManager>();

            var successedId = dbWorker.ConfigsNumber() + 1;
            var failedId = successedId + 1;
            var testTenant = new Tenant(2, "testWebhooksSender");

            tenantManager.SetCurrentTenant(testTenant);

            var successWebhookConfig = new WebhooksConfig { ConfigId = successedId, SecretKey = secretKey, Uri = $"{URI}SuccessRequest/" };
            var failedWebhookConfig = new WebhooksConfig { ConfigId = failedId, SecretKey = secretKey, Uri = $"{URI}FailedRequest/" };
            dbWorker.AddWebhookConfig(successWebhookConfig);
            dbWorker.AddWebhookConfig(failedWebhookConfig);

            var successWebhookPayload = new WebhooksPayload { ConfigId = successedId, Status = ProcessStatus.InProcess, CreationTime = creationTime, Data = Content, TenantId = testTenant.TenantId };
            var failedWebhookPayload = new WebhooksPayload { ConfigId = failedId, Status = ProcessStatus.InProcess, CreationTime = creationTime, Data = Content, TenantId = testTenant.TenantId };
            var successWebhookPayloadId = dbWorker.WriteToJournal(successWebhookPayload);
            var failedWebhookPayloadId = dbWorker.WriteToJournal(failedWebhookPayload);

            var mockedLog = new Mock<ILog>();
            mockedLog.Setup(a => a.Error(It.IsAny<string>())).Verifiable();

            var mockedLogOptions = new Mock<IOptionsMonitor<ILog>>();
            mockedLogOptions.Setup(a => a.Get("ASC.Webhooks.Core")).Returns(mockedLog.Object).Verifiable();

            var source = new CancellationTokenSource();
            var token = source.Token;

            var SuccessedWebhookRequest = new WebhookRequest { Id = successWebhookPayloadId };
            var FailedWebhookRequest = new WebhookRequest { Id = failedWebhookPayloadId };

            var sender = new WebhookSender(mockedLogOptions.Object, serviceProvider, settings);
            await sender.Send(SuccessedWebhookRequest, token);
            await sender.Send(FailedWebhookRequest, token);

            var asd = requestHistory.SuccessCounter;

            Assert.IsTrue(requestHistory.SuccessCounter == 1, "Problem with successed request");
            Assert.IsTrue(requestHistory.FailedCounter == webhookSender.RepeatCount, "Problem with failed request");
            Assert.IsTrue(requestHistory.ÑorrectSignature, "Problem with signature");
        }

        [Test]
        public async Task Middleware()
        {
            try
            {
                var controllerAddress = "api/2.0/Test/testMethod";
                var getEventName = $"method: GET, route: {controllerAddress}";
                var postEventName = $"method: POST, route: {controllerAddress}";

                var mockedWebhookPubslisher = new Mock<IWebhookPublisher>();
                mockedWebhookPubslisher.Setup(a => a.Publish(getEventName, Content)).Verifiable();
                mockedWebhookPubslisher.Setup(a => a.Publish(postEventName, Content)).Verifiable();
              

                using var host = await new HostBuilder()
                    .ConfigureWebHost(webBuilder =>
                    {
                        webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(mockedWebhookPubslisher.Object);
                            services.AddControllers();

                            var dIHelper = new DIHelper();
                            dIHelper.Configure(services);
                            dIHelper.TryAdd<TestController>();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseWebhooksMiddleware();

                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                                endpoints.MapCustom();
                            });
                        });
                    })
                    .StartAsync();

                var getResponse = await host.GetTestClient().GetAsync(controllerAddress);
                mockedWebhookPubslisher.Verify(a => a.Publish(getEventName, Content), Times.Never);

                StringContent stringContent = new StringContent(Content);

                var postResponse = await host.GetTestClient().PostAsync(controllerAddress, stringContent);
                mockedWebhookPubslisher.Verify(a => a.Publish(postEventName, Content), Times.Once);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            Assert.Pass();
        }
    }
}