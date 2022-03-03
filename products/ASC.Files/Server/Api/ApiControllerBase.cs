namespace ASC.Files.Api;

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("files")]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly FilesControllerHelper<int> _filesControllerHelperInt;
    protected readonly FilesControllerHelper<string> _filesControllerHelperString;

    public ApiControllerBase(FilesControllerHelper<int> filesControllerHelperInt, FilesControllerHelper<string> filesControllerHelperString)
    {
        _filesControllerHelperInt = filesControllerHelperInt;
        _filesControllerHelperString = filesControllerHelperString;
    }
}
