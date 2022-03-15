namespace ASC.Core.Users;

public static class UserExtensions
{
    public static bool IsOwner(this UserInfo ui, Tenant tenant)
    {
        if (ui == null)
        {
            return false;
        }

        return tenant != null && tenant.OwnerId.Equals(ui.Id);
    }

    public static bool IsMe(this UserInfo ui, AuthContext authContext)
    {
        return ui != null && ui.Id == authContext.CurrentAccount.ID;
    }

    public static bool IsAdmin(this UserInfo ui, UserManager UserManager)
    {
        return ui != null && UserManager.IsUserInGroup(ui.Id, Constants.GroupAdmin.ID);
    }

    public static bool IsVisitor(this UserInfo ui, UserManager UserManager)
    {
        return ui != null && UserManager.IsUserInGroup(ui.Id, Constants.GroupVisitor.ID);
    }

    public static bool IsOutsider(this UserInfo ui, UserManager userManager)
    {
        return IsVisitor(ui, userManager) && ui.Id == Constants.OutsideUser.Id;
    }

    public static bool IsLDAP(this UserInfo ui)
    {
        if (ui == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(ui.Sid);
    }

    // ReSharper disable once InconsistentNaming
    public static bool IsSSO(this UserInfo ui)
    {
        if (ui == null)
        {
            return false;
        }

        return !string.IsNullOrEmpty(ui.SsoNameId);
    }

    private const string ExtMobPhone = "extmobphone";
    private const string MobPhone = "mobphone";
    private const string ExtMail = "extmail";
    private const string Mail = "mail";

    public static void ConvertExternalContactsToOrdinary(this UserInfo ui)
    {
        var ldapUserContacts = ui.ContactsList;

        if (ui.ContactsList == null)
        {
            return;
        }

        var newContacts = new List<string>();

        for (int i = 0, m = ldapUserContacts.Count; i < m; i += 2)
        {
            if (i + 1 >= ldapUserContacts.Count)
            {
                continue;
            }

            var type = ldapUserContacts[i];
            var value = ldapUserContacts[i + 1];

            switch (type)
            {
                case ExtMobPhone:
                    newContacts.Add(MobPhone);
                    newContacts.Add(value);
                    break;
                case ExtMail:
                    newContacts.Add(Mail);
                    newContacts.Add(value);
                    break;
                default:
                    newContacts.Add(type);
                    newContacts.Add(value);
                    break;
            }
        }

        ui.ContactsList = newContacts;
    }
}
