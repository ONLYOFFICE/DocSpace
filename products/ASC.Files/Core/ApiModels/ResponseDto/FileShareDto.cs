namespace ASC.Files.Core.ApiModels.ResponseDto;

public class FileShareDto
{
    public FileShareDto() { }

    public FileShare Access { get; set; }
    public object SharedTo { get; set; }
    public bool IsLocked { get; set; }
    public bool IsOwner { get; set; }

    public static FileShareDto GetSample()
    {
        return new FileShareDto
        {
            Access = FileShare.ReadWrite,
            IsLocked = false,
            IsOwner = true,
            //SharedTo = EmployeeWraper.GetSample()
        };
    }
}

public class FileShareLink
{
    public Guid Id { get; set; }
    public string ShareLink { get; set; }
}

[Scope]
public class FileShareDtoHelper
{
    private readonly UserManager _userManager;
    private readonly EmployeeFullDtoHelper _employeeWraperFullHelper;

    public FileShareDtoHelper(
        UserManager userManager,
        EmployeeFullDtoHelper employeeWraperFullHelper)
    {
        _userManager = userManager;
        _employeeWraperFullHelper = employeeWraperFullHelper;
    }

    public FileShareDto Get(AceWrapper aceWrapper)
    {
        var result = new FileShareDto
        {
            IsOwner = aceWrapper.Owner,
            IsLocked = aceWrapper.LockedRights
        };

        if (aceWrapper.SubjectGroup)
        {
            if (aceWrapper.SubjectId == FileConstant.ShareLinkId)
            {
                result.SharedTo = new FileShareLink
                {
                    Id = aceWrapper.SubjectId,
                    ShareLink = aceWrapper.Link
                };
            }
            else
            {
                //Shared to group
                result.SharedTo = new GroupSummaryDto(_userManager.GetGroupInfo(aceWrapper.SubjectId), _userManager);
            }
        }
        else
        {
            result.SharedTo = _employeeWraperFullHelper.GetFull(_userManager.GetUsers(aceWrapper.SubjectId));
        }

        result.Access = aceWrapper.Share;

        return result;
    }
}
