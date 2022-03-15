namespace ASC.Web.Files.Utils;

[Scope]
public class SocketManager
{
    private readonly SignalrServiceClient _signalrServiceClient;
    private readonly FileDtoHelper _filesWrapperHelper;
    private readonly TenantManager _tenantManager;
    public IDaoFactory DaoFactory { get; }

    public SocketManager(
        IOptionsSnapshot<SignalrServiceClient> optionsSnapshot,
        FileDtoHelper filesWrapperHelper,
        TenantManager tenantManager,
        IDaoFactory daoFactory
        )
    {
        _signalrServiceClient = optionsSnapshot.Get("files");
        _filesWrapperHelper = filesWrapperHelper;
        _tenantManager = tenantManager;
        DaoFactory = daoFactory;
    }

    public void StartEdit<T>(T fileId)
    {
        var room = GetFileRoom(fileId);
        _signalrServiceClient.StartEdit(fileId, room);
    }

    public async Task StopEditAsync<T>(T fileId)
    {
        var room = GetFileRoom(fileId);
        var file = await DaoFactory.GetFileDao<T>().GetFileStableAsync(fileId);

        var serializerSettings = new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        serializerSettings.Converters.Add(new ApiDateTimeConverter());
        serializerSettings.Converters.Add(new FileEntryWrapperConverter());
        var data = JsonSerializer.Serialize(await _filesWrapperHelper.GetAsync(file), serializerSettings);

        _signalrServiceClient.StopEdit(fileId, room, data);
    }

    public async Task CreateFileAsync<T>(File<T> file)
    {
        var room = GetFolderRoom(file.FolderID);
        var serializerSettings = new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        serializerSettings.Converters.Add(new ApiDateTimeConverter());
        serializerSettings.Converters.Add(new FileEntryWrapperConverter());
        var data = JsonSerializer.Serialize(await _filesWrapperHelper.GetAsync(file), serializerSettings);

        _signalrServiceClient.CreateFile(file.ID, room, data);
    }

    public void DeleteFile<T>(File<T> file)
    {
        var room = GetFolderRoom(file.FolderID);
        _signalrServiceClient.DeleteFile(file.ID, room);
    }

    private string GetFileRoom<T>(T fileId)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return $"{tenantId}-FILE-{fileId}";
    }

    private string GetFolderRoom<T>(T folderId)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return $"{tenantId}-DIR-{folderId}";
    }
}
