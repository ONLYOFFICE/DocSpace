using Profile = AutoMapper.Profile;

namespace ASC.Files.Core.Core.Entries;

public class FileWithShare : IMapFrom<DbFileQueryWithSecurity>
{
    public File<int> File { get; set; }
    public SmallShareRecord ShareRecord { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbFileQueryWithSecurity, FileWithShare>()
           .ForMember(r => r.File, r => r.MapFrom(s => s.DbFileQuery))
           .ForMember(r => r.ShareRecord, r => r.MapFrom(s => s.Security));

        profile.CreateMap<DbFilesSecurity, SmallShareRecord>();
    }
}