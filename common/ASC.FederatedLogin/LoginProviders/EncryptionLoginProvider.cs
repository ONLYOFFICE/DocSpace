using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Studio.Core;

[Scope]
public class EncryptionLoginProvider
{
    private readonly SecurityContext _securityContext;
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly IOptionsSnapshot<AccountLinker> _snapshot;

    public EncryptionLoginProvider(
        SecurityContext securityContext,
        Signature signature,
        InstanceCrypto instanceCrypto,
        IOptionsSnapshot<AccountLinker> snapshot)
    {
        _securityContext = securityContext;
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _snapshot = snapshot;
    }


    public void SetKeys(Guid userId, string keys)
    {
        if (string.IsNullOrEmpty(keys))
        {
            return;
        }

        var loginProfile = new LoginProfile(_signature, _instanceCrypto)
        {
            Provider = ProviderConstants.Encryption,
            Name = _instanceCrypto.Encrypt(keys)
        };

        var linker = _snapshot.Get("webstudio");
        linker.AddLink(userId.ToString(), loginProfile);
    }

    public string GetKeys()
    {
        return GetKeys(_securityContext.CurrentAccount.ID);
    }

    public string GetKeys(Guid userId)
    {
        var linker = _snapshot.Get("webstudio");
        var profile = linker.GetLinkedProfiles(userId.ToString(), ProviderConstants.Encryption).FirstOrDefault();
        if (profile == null)
        {
            return null;
        }

        var keys = _instanceCrypto.Decrypt(profile.Name);

        return keys;
    }
}
