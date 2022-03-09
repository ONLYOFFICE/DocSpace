using Profile = AutoMapper.Profile;

namespace ASC.Files.Core.Core.Entries;

public class FileWithShare : IMapFrom<DbFileQueryWithSecurity>
{
    public File<int> File { get; set; }
    public SmallShareRecord ShareRecord { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbFileQueryWithSecurity, FileWithShare>()
            .ConvertUsing<FilesTypeConverter>();
    }
}