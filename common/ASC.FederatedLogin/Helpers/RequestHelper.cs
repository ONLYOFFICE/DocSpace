namespace ASC.FederatedLogin.Helpers;

public static class RequestHelper
{
    private readonly static HttpClient _httpClient = new HttpClient();

    public static string PerformRequest(string uri, string contentType = "", string method = "GET", string body = "", Dictionary<string, string> headers = null, int timeout = 30000)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(uri);

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(uri);
        request.Method = new HttpMethod(method);

        _httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);

        if (headers != null)
        {
            foreach (var key in headers.Keys)
            {
                request.Headers.Add(key, headers[key]);
            }
        }

        var bytes = Encoding.UTF8.GetBytes(body ?? "");
        if (request.Method != HttpMethod.Get && bytes.Length > 0)
        {
            request.Content = new ByteArrayContent(bytes, 0, bytes.Length);
            if (!string.IsNullOrEmpty(contentType))
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
        }

        using var response = _httpClient.Send(request);
        using var stream = response.Content.ReadAsStream();
        if (stream == null)
        {
            return null;
        }

        using var readStream = new StreamReader(stream);

        return readStream.ReadToEnd();
    }
}
