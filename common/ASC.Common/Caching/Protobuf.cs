using System;
using Confluent.Kafka;
using Google.Protobuf;

namespace ASC.Common.Caching
{
    public class ProtobufSerializer<T> : ISerializer<T> where T : IMessage<T>, new()
    {
        public byte[] Serialize(T data, SerializationContext context)
            => data.ToByteArray();
    }

    public class ProtobufDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
    {
        private readonly MessageParser<T> parser;

        public ProtobufDeserializer()
        {
            parser = new MessageParser<T>(() => new T());
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            => parser.ParseFrom(data.ToArray());
    }
}
