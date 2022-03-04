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
    public async Task<object> CheckFillFormDraftFromBodyAsync(string fileId, [FromBody] CheckFillFormDraftRequestDto requestDto)
    {
        return await _filesControllerHelperString.CheckFillFormDraftAsync(fileId, requestDto.Version, requestDto.Doc, !requestDto.RequestEmbedded, requestDto.RequestView);
    }

    [Create("masterform/{fileId}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(string fileId, [FromForm] CheckFillFormDraftRequestDto requestDto)
    {
        return await _filesControllerHelperString.CheckFillFormDraftAsync(fileId, requestDto.Version, requestDto.Doc, !requestDto.RequestEmbedded, requestDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    public async Task<object> CheckFillFormDraftFromBodyAsync(int fileId, [FromBody] CheckFillFormDraftRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CheckFillFormDraftAsync(fileId, requestDto.Version, requestDto.Doc, !requestDto.RequestEmbedded, requestDto.RequestView);
    }

    [Create("masterform/{fileId:int}/checkfillformdraft")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<object> CheckFillFormDraftFromFormAsync(int fileId, [FromForm] CheckFillFormDraftRequestDto requestDto)
    {
        return await _filesControllerHelperInt.CheckFillFormDraftAsync(fileId, requestDto.Version, requestDto.Doc, !requestDto.RequestEmbedded, requestDto.RequestView);
    }
}