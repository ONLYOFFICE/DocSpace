using ConfigurationConstants = ASC.Core.Configuration.Constants;
using UserConstants = ASC.Core.Users.Constants;

namespace ASC.Data.Backup.Tasks.Modules;

[Scope]
public class Helpers
{
    private readonly InstanceCrypto _instanceCrypto;

    private readonly Guid[] _systemUsers = new[]
    {
            Guid.Empty,
            ConfigurationConstants.CoreSystem.ID,
            ConfigurationConstants.Guest.ID,
            UserConstants.LostUser.Id
        };

    private readonly Guid[] _systemGroups = new[]
    {
            Guid.Empty,
            UserConstants.LostGroupInfo.ID,
            UserConstants.GroupAdmin.ID,
            UserConstants.GroupEveryone.ID,
            UserConstants.GroupVisitor.ID,
            UserConstants.GroupUser.ID,
            new Guid("{EA942538-E68E-4907-9394-035336EE0BA8}"), //community product
            new Guid("{1e044602-43b5-4d79-82f3-fd6208a11960}"), //projects product
            new Guid("{6743007C-6F95-4d20-8C88-A8601CE5E76D}"), //crm product
            new Guid("{E67BE73D-F9AE-4ce1-8FEC-1880CB518CB4}"), //documents product
            new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}"), //people product
            new Guid("{2A923037-8B2D-487b-9A22-5AC0918ACF3F}"), //mail product
            new Guid("{32D24CB5-7ECE-4606-9C94-19216BA42086}"), //calendar product
            new Guid("{37620AE5-C40B-45ce-855A-39DD7D76A1FA}"), //birthdays product
            new Guid("{BF88953E-3C43-4850-A3FB-B1E43AD53A3E}")  //talk product
        };

    public Helpers(InstanceCrypto instanceCrypto)
    {
        _instanceCrypto = instanceCrypto;
    }

    public bool IsEmptyOrSystemUser(string id)
    {
        return string.IsNullOrEmpty(id) || Guid.TryParse(id, out var g) && _systemUsers.Contains(g);
    }

    public bool IsEmptyOrSystemGroup(string id)
    {
        return string.IsNullOrEmpty(id) || Guid.TryParse(id, out var g) && _systemGroups.Contains(g);
    }

    public string CreateHash(string s)
    {
        return !string.IsNullOrEmpty(s) && s.StartsWith("S|") ? _instanceCrypto.Encrypt(Crypto.GetV(s.Substring(2), 1, false)) : s;
    }

    public string CreateHash2(string s)
    {
        return !string.IsNullOrEmpty(s) ? "S|" + Crypto.GetV(_instanceCrypto.Decrypt(s), 1, true) : s;
    }
}
