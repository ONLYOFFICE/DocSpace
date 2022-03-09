namespace ASC.Files.Core.Mapping;

[Scope]
public class FoldersTypeConverter: ITypeConverter<DbFolderQuery, Folder<int>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantUtil _tenantUtil;

    public FoldersTypeConverter(IServiceProvider serviceProvider, TenantUtil tenantUtil)
    {
        _serviceProvider = serviceProvider;
        _tenantUtil = tenantUtil;
    }

    public Folder<int> Convert(DbFolderQuery source, Folder<int> destination, ResolutionContext context)
    {
        var result = _serviceProvider.GetService<Folder<int>>();

        _ = context.Mapper.Map(source.Folder, result);

        result.CreateOn = _tenantUtil.DateTimeFromUtc(source.Folder.CreateOn);
        result.ModifiedOn = _tenantUtil.DateTimeFromUtc(source.Folder.ModifiedOn);
        result.RootFolderType = source.Root?.FolderType ?? default;
        result.RootFolderCreator = source.Root?.CreateBy ?? default;
        result.RootFolderId = source.Root?.Id ?? default;
        result.Shared = source.Shared;

        switch (result.FolderType)
        {
            case FolderType.COMMON:
                result.Title = FilesUCResource.CorporateFiles;
                break;
            case FolderType.USER:
                result.Title = FilesUCResource.MyFiles;
                break;
            case FolderType.SHARE:
                result.Title = FilesUCResource.SharedForMe;
                break;
            case FolderType.Recent:
                result.Title = FilesUCResource.Recent;
                break;
            case FolderType.Favorites:
                result.Title = FilesUCResource.Favorites;
                break;
            case FolderType.TRASH:
                result.Title = FilesUCResource.Trash;
                break;
            case FolderType.Privacy:
                result.Title = FilesUCResource.PrivacyRoom;
                break;
            case FolderType.Projects:
                result.Title = FilesUCResource.ProjectFiles;
                break;
            case FolderType.BUNCH:
                try
                {
                    result.Title = string.Empty;
                }
                catch (Exception)
                {
                    //Global.Logger.Error(e);
                }
                break;
        }

        if (result.FolderType != FolderType.DEFAULT && 0.Equals(result.ParentId))
        {
            result.RootFolderType = result.FolderType;
        }

        if (result.FolderType != FolderType.DEFAULT && result.RootFolderCreator == default)
        {
            result.RootFolderCreator = result.CreateBy;
        }

        if (result.FolderType != FolderType.DEFAULT && 0.Equals(result.RootFolderId))
        {
            result.RootFolderId = result.Id;
        }

        return result;
    }
}