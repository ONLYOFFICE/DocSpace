namespace ASC.Files.Core.ApiModels;

public class FileShareParams
{
    public Guid ShareTo { get; set; }
    public FileShare Access { get; set; }
}

[Scope]
public class FileShareParamsHelper
{
    private readonly UserManager _userManager;

    public FileShareParamsHelper(UserManager userManager)
    {
        _userManager = userManager;
    }

    public AceWrapper ToAceObject(FileShareParams fileShareParams)
    {
        return new AceWrapper
        {
            Share = fileShareParams.Access,
            SubjectId = fileShareParams.ShareTo,
            SubjectGroup = !_userManager.UserExists(fileShareParams.ShareTo)
        };
    }
}
