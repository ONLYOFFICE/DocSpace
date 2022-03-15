namespace ASC.Web.Core.Users
{
    public class UserInfoComparer : IComparer<UserInfo>
    {
        public static readonly IComparer<UserInfo> Default = new UserInfoComparer(UserSortOrder.DisplayName, false);
        public static readonly IComparer<UserInfo> FirstName = new UserInfoComparer(UserSortOrder.FirstName, false);
        public static readonly IComparer<UserInfo> LastName = new UserInfoComparer(UserSortOrder.LastName, false);


        public UserSortOrder SortOrder { get; set; }
        public bool Descending { get; set; }


        public UserInfoComparer(UserSortOrder sortOrder)
            : this(sortOrder, false)
        {
        }

        public UserInfoComparer(UserSortOrder sortOrder, bool descending)
        {
            SortOrder = sortOrder;
            Descending = descending;
        }


        public int Compare(UserInfo x, UserInfo y)
        {
            var result = 0;
            switch (SortOrder)
            {
                case UserSortOrder.DisplayName:
                    result = UserFormatter.Compare(x, y, DisplayUserNameFormat.Default);
                    break;
                case UserSortOrder.FirstName:
                    result = UserFormatter.Compare(x, y, DisplayUserNameFormat.FirstLast);
                    break;
                case UserSortOrder.LastName:
                    result = UserFormatter.Compare(x, y, DisplayUserNameFormat.LastFirst);
                    break;
            }

            return !Descending ? result : -result;
        }
    }
}
