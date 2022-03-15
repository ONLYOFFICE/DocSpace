namespace ASC.Web.Api.ApiModel.ResponseDto;

public class StorageDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public List<AuthKey> Properties { get; set; }
    public bool Current { get; set; }
    public bool IsSet { get; set; }

    public StorageDto(DataStoreConsumer consumer, StorageSettings current)
    {
        StorageWrapperInit(consumer, current);
    }

    public StorageDto(DataStoreConsumer consumer, CdnStorageSettings current)
    {
        StorageWrapperInit(consumer, current);
    }

    private void StorageWrapperInit<T>(DataStoreConsumer consumer, BaseStorageSettings<T> current) where T : class, ISettings, new()
    {
        Id = consumer.Name;
        Title = ConsumerExtension.GetResourceString(consumer.Name) ?? consumer.Name;
        Current = consumer.Name == current.Module;
        IsSet = consumer.IsSet;

        var props = Current
            ? current.Props
            : current.Switch(consumer).AdditionalKeys.ToDictionary(r => r, a => consumer[a]);

        Properties = props.Select(
            r => new AuthKey
            {
                Name = r.Key,
                Value = r.Value,
                Title = ConsumerExtension.GetResourceString(consumer.Name + r.Key) ?? r.Key
            }).ToList();
    }
}