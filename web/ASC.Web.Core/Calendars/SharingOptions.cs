using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Core.Calendars
{
    public class SharingOptions : ICloneable
    {
        public class PublicItem
        {
            public Guid Id { get; set; }
            public bool IsGroup { get; set; }
        }

        public bool SharedForAll { get; set; }

        public List<PublicItem> PublicItems { get; set; }

        public SharingOptions()
        {
            this.PublicItems = new List<PublicItem>();
        }

        public bool PublicForItem(Guid itemId, UserManager userManager)
        {
            if (SharedForAll)
                return true;

            if (PublicItems.Exists(i => i.Id.Equals(itemId)))
                return true;

            var u = userManager.GetUsers(itemId);
            if (u != null && u.Id != ASC.Core.Users.Constants.LostUser.Id)
            {
                var userGroups = new List<GroupInfo>(userManager.GetUserGroups(itemId));
                userGroups.AddRange(userManager.GetUserGroups(itemId, Constants.SysGroupCategoryId));
                return userGroups.Exists(g => PublicItems.Exists(i => i.Id.Equals(g.ID)));
            }

            return false;
        }

        #region ICloneable Members

        public object Clone()
        {
            var o = new SharingOptions
            {
                SharedForAll = this.SharedForAll
            };
            foreach (var i in this.PublicItems)
                o.PublicItems.Add(new PublicItem() { Id = i.Id, IsGroup = i.IsGroup });

            return o;
        }

        #endregion
    }


}
