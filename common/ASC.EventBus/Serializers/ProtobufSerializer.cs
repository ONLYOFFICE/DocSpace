namespace ASC.EventBus.Serializers;

public class ProtobufSerializer : IIntegrationEventSerializer
{
    private SynchronizedCollection<string> _processedProtoTypes;
    private readonly int _baseFieldNumber;
    
    public ProtobufSerializer()
    {
        _processedProtoTypes = new SynchronizedCollection<string>();
        _baseFieldNumber = 100;
    }

    /// <inheritdoc/>
    public byte[] Serialize<T>(T? item)
    {
        if (item == null)
            return Array.Empty<byte>();
        
        ProcessProtoType(item.GetType());

        using var ms = new MemoryStream();

        Serializer.Serialize(ms, item);

        return ms.ToArray();
    }

    /// <inheritdoc/>
    public T Deserialize<T>(byte[] serializedObject)
    {
      //  ProcessProtoType(returnType);

        using var ms = new MemoryStream(serializedObject);

        return Serializer.Deserialize<T>(ms);
    }

    /// <inheritdoc/>
    public object Deserialize(byte[] serializedObject, Type returnType)
    {
        ProcessProtoType(returnType);

        using var ms = new MemoryStream(serializedObject);

        return Serializer.Deserialize(returnType, ms);
    }

    private void ProcessProtoType(Type protoType)
    {
        if (_processedProtoTypes.Contains(protoType.FullName)) return;

        if (protoType.BaseType == null && protoType.BaseType == typeof(object)) return;               

        var itemType = RuntimeTypeModel.Default[protoType];
                
        var baseType = RuntimeTypeModel.Default[protoType.BaseType];
        
        if (!baseType.GetSubtypes().Any(s => s.DerivedType == itemType))
        {
            baseType.AddSubType(_baseFieldNumber, protoType);

            //foreach (var field in baseType.GetFields())
            //{
            //    myType.Add(field.FieldNumber + _baseTypeIncrement, field.Name);
            //}
        }

        _processedProtoTypes.Add(protoType.FullName);
    }

}
