namespace ASC.Core.ChunkedUploader;

[Serializable]
public class CommonChunkedUploadSession : ICloneable
{
    public string Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expired { get; set; }
    public string Location { get; set; }
    public long BytesUploaded { get; set; }
    public long BytesTotal { get; set; }
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool UseChunks { get; set; }
    public string CultureName { get; set; }
    public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();

    [JsonIgnore]
    public string TempPath
    {
        get => GetItemOrDefault<string>(TempPathKey);
        set => Items[TempPathKey] = value;
    }

    [JsonIgnore]
    public string UploadId
    {
        get => GetItemOrDefault<string>(UploadIdKey);
        set => Items[UploadIdKey] = value;
    }

    [JsonIgnore]
    public string ChunksBuffer
    {
        get => GetItemOrDefault<string>(ChunksBufferKey);
        set => Items[ChunksBufferKey] = value;
    }

    private const string TempPathKey = "TempPath";
    private const string UploadIdKey = "UploadId";
    private const string ChunksBufferKey = "ChunksBuffer";

    public CommonChunkedUploadSession(long bytesTotal)
    {
        Id = Guid.NewGuid().ToString("N");
        Created = DateTime.UtcNow;
        BytesUploaded = 0;
        BytesTotal = bytesTotal;
        UseChunks = true;
    }

    public T GetItemOrDefault<T>(string key)
    {
        return Items.ContainsKey(key) && Items[key] is T t ? t : default;
    }

    public virtual Stream Serialize()
    {
        return null;
    }

    public void TransformItems()
    {
        var newItems = new Dictionary<string, object>();

        foreach (var item in Items)
        {
            if (item.Value != null)
            {
                    if (item.Value is JsonElement)
                {
                    var value = (JsonElement)item.Value;
                    if (value.ValueKind == JsonValueKind.String)
                    {
                        newItems.Add(item.Key, item.Value.ToString());
                    }
                    if (value.ValueKind == JsonValueKind.Number)
                    {
                        newItems.Add(item.Key, Int32.Parse(item.Value.ToString()));
                    }
                    if (value.ValueKind == JsonValueKind.Array)
                    {
                        newItems.Add(item.Key, value.EnumerateArray().Select(o => o.ToString()).ToList());
                    }
                }
                else
                {
                    newItems.Add(item.Key, item.Value);
                }
            }
        }
        Items = newItems;
    }

    public virtual object Clone()
    {
        return (CommonChunkedUploadSession)MemberwiseClone();
    }
}
