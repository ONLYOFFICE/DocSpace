namespace ASC.Files.Api;

public class MasterFormController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public MasterFormController(FileStorageService<int> fileStorageServiceInt, FileStorageService<string> fileStorageServiceString)
    {
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageServiceString = fileStorageServiceString;
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(string fileId, [FromBody] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageServiceString.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(string fileId, [FromForm] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageServiceString.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(int fileId, [FromBody] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageServiceInt.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(int fileId, [FromForm] CheckFillFormDraftRequestDto inDto)
    {
        return await _fileStorageServiceInt.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }
}