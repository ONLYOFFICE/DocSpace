using Profile = AutoMapper.Profile;

namespace ASC.Files.Core.Core.Entries;

public class FolderWithShare : IMapFrom<DbFolderQueryWithSecurity>
{
    public Folder<int> Folder { get; set; }
    public SmallShareRecord ShareRecord { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbFolderQueryWithSecurity, FolderWithShare>()
            .ConvertUsing<FoldersTypeConverter>();
    }
}