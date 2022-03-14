namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class MasterFormControllerInternal : MasterFormController<int>
{
    public MasterFormControllerInternal(FileStorageService<int> fileStorageServiceString) : base(fileStorageServiceString)
    {
    }
}

public class MasterFormControllerThirdparty : MasterFormController<string>
{
    public MasterFormControllerThirdparty(FileStorageService<string> fileStorageServiceString) : base(fileStorageServiceString)
    {
    }
}

public abstract class MasterFormController<T> : ApiControllerBase
{
    private readonly FileStorageService<T> _fileStorageService;

    public MasterFormController(FileStorageService<T> fileStorageServiceString)
    {
        _fileStorageService = fileStorageServiceString;
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(T fileId, [FromBody] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageService.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(T fileId, [FromForm] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageService.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }
}