namespace ASC.Core;

public class Group : IMapFrom<DbGroup>
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }
    public int Tenant { get; set; }
    public string Sid { get; set; }

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Group g && g.Id == Id;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbGroup, Group>()
            .ForMember(src => src.CategoryId, opt => opt.NullSubstitute(Guid.Empty))
            .ForMember(src => src.ParentId, opt => opt.NullSubstitute(Guid.Empty));

        profile.CreateMap<GroupInfo, Group>();
    }
}
