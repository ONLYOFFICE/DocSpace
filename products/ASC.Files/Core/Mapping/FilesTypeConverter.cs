namespace ASC.Files.Core.Mapping;

[Scope]
public class FilesTypeConverter : ITypeConverter<DbFileQuery, File<int>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantUtil _tenantUtil;

    public FilesTypeConverter(IServiceProvider serviceProvider, TenantUtil tenantUtil)
    {
        _serviceProvider = serviceProvider;
        _tenantUtil = tenantUtil;
    }

    public File<int> Convert(DbFileQuery source, File<int> destination, ResolutionContext context)
    {
        if (source == null)
        {
            return null;
        }

        var file = _serviceProvider.GetService<File<int>>();

        _ = context.Mapper.Map(source.File, file);

        file.CreateOn = _tenantUtil.DateTimeFromUtc(source.File.CreateOn);
        file.ModifiedOn = _tenantUtil.DateTimeFromUtc(source.File.ModifiedOn);
        file.Shared = source.Shared;
        file.IsFillFormDraft = source.Linked;
        file.RootFolderType = source.Root?.FolderType ?? default;
        file.RootFolderCreator = source.Root?.CreateBy ?? default;
        file.RootFolderId = source.Root?.Id ?? default;

        return file;
    }
}
