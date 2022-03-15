namespace ASC.Common.Web;

public abstract class AbstractHttpAsyncHandler // : IHttpAsyncHandler, IReadOnlySessionState
{
    public bool IsReusable => false;

    private Action<HttpContext> _processRequest;
    private IPrincipal _principal;
    private CultureInfo _culture;

    public void ProcessRequest(HttpContext context)
    {
        Thread.CurrentThread.CurrentCulture = _culture;
        Thread.CurrentThread.CurrentUICulture = _culture;
        Thread.CurrentPrincipal = _principal;
        //HttpContext.Current = context;
        OnProcessRequest(context);
    }

    public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
    {
        _culture = Thread.CurrentThread.CurrentCulture;
        _principal = Thread.CurrentPrincipal;
        _processRequest = ProcessRequest;

        return _processRequest.BeginInvoke(context, cb, extraData);
    }

    public void EndProcessRequest(IAsyncResult result)
    {
        _processRequest.EndInvoke(result);
    }

    public abstract void OnProcessRequest(HttpContext context);
}
