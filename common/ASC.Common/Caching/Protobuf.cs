namespace ASC.Common.Caching;

public class ProtobufSerializer<T> : ISerializer<T> where T : IMessage<T>, new()
{
    public byte[] Serialize(T data, SerializationContext context) => data.ToByteArray();
}

public class ProtobufDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    private readonly MessageParser<T> _parser;

    public ProtobufDeserializer() => _parser = new MessageParser<T>(() => new T());

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        => _parser.ParseFrom(data.ToArray());
}

public static class GuidExtension
{
    public static ByteString ToByteString(this Guid id) => ByteString.CopyFrom(id.ToByteArray());

    public static Guid FromByteString(this ByteString id) => new Guid(id.ToByteArray());
}