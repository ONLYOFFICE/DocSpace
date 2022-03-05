namespace ASC.Files.Api;

public class MasterFormController : ApiControllerBase
{
    public MasterFormController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(string fileId, [FromBody] CheckFillFormDraftRequestDto inDto)
    {
        return await _filesControllerHelperString.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(string fileId, [FromForm] CheckFillFormDraftRequestDto inDto)
    {
        return await _filesControllerHelperString.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(int fileId, [FromBody] CheckFillFormDraftRequestDto inDto)
    {
        return await _filesControllerHelperInt.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(int fileId, [FromForm] CheckFillFormDraftRequestDto inDto)
    {
        return await _filesControllerHelperInt.CheckFillFormDraftAsync(fileId, inDto.Version, inDto.Doc, !inDto.RequestEmbedded, inDto.RequestView);
    }
}