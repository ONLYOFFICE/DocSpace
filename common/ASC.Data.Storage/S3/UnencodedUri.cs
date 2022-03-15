namespace ASC.Data.Storage.S3;

public class UnencodedUri : Uri
{
    public UnencodedUri(string uriString)
        : base(uriString) { }

    public UnencodedUri(string uriString, UriKind uriKind)
        : base(uriString, uriKind) { }

    public UnencodedUri(Uri baseUri, string relativeUri)
        : base(baseUri, relativeUri) { }

    public UnencodedUri(Uri baseUri, Uri relativeUri)
        : base(baseUri, relativeUri) { }

    protected UnencodedUri(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext) { }

    public override string ToString()
    {
        return OriginalString;
    }
}
