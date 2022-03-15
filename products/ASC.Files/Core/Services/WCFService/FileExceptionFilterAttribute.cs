namespace ASC.Web.Files.Services.WCFService;

class FileExceptionFilterAttribute : IExceptionFilter
{
    private readonly ILog _logger;

    public FileExceptionFilterAttribute(IOptionsMonitor<ILog> options)
    {
        _logger = options.Get("ASC.Files");
    }

    public void OnException(ExceptionContext actionExecutedContext)
    {
        if (actionExecutedContext.Exception != null)
        {
            var fileError = new FileError(actionExecutedContext.Exception);
            actionExecutedContext.Result = new ObjectResult(fileError)
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        LogException(actionExecutedContext.Exception);
    }


    [Conditional("DEBUG")]
    private void LogException(Exception err)
    {
        while (err != null)
        {
            _logger.Error(err);
            err = err.InnerException;
        }
    }

    class FileError
    {
        public string Detail { get; set; }
        public string Message { get; set; }
        public FileErrorInner Inner { get; set; }

        internal class FileErrorInner
        {
            public string Message { get; set; }
            public string Type { get; set; }
            public string Source { get; set; }
            public string Stack { get; set; }
        }

        public FileError() { }

        public FileError(Exception error)
        {
            Detail = error.Message;
            Message = FilesCommonResource.ErrorMassage_BadRequest;
            Inner = new FileErrorInner
            {
                Message = error.Message,
                Type = error.GetType().FullName,
                Source = error.Source ?? string.Empty,
                Stack = error.StackTrace ?? string.Empty,
            };
        }
    }
}
