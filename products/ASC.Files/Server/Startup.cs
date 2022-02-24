namespace ASC.Files
{
    public class Startup : BaseStartup
    {
        public override JsonConverter[] Converters { get => new JsonConverter[] { new FileEntryWrapperConverter(), new FileShareConverter() }; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddMemoryCache();

            base.ConfigureServices(services);

            DIHelper.TryAdd<FilesController>();
            DIHelper.TryAdd<PrivacyRoomController>();
            DIHelper.TryAdd<FileHandlerService>();
            DIHelper.TryAdd<ChunkedUploaderHandlerService>();
            DIHelper.TryAdd<DocuSignHandlerService>();
            DIHelper.TryAdd<ThirdPartyAppHandlerService>();

            NotifyConfigurationExtension.Register(DIHelper);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            base.Configure(app, env);

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/filehandler.ashx"),
                appBranch =>
                {
                    appBranch.UseFileHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ChunkedUploader.ashx"),
                appBranch =>
                {
                    appBranch.UseChunkedUploaderHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ThirdPartyAppHandler.ashx"),
                appBranch =>
                {
                    appBranch.UseThirdPartyAppHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("DocuSignHandler.ashx"),
                appBranch =>
                {
                    appBranch.UseDocuSignHandler();
                });
        }
    }
}
