/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System.Collections.Generic;

using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Files.Services.DocumentService;

namespace ASC.Web.Files.Services.WCFService
{
    public class MentionWrapper
    {
        public string Email
        {
            get { return User.Email; }
        }

        public string Id
        {
            get { return User.ID.ToString(); }
        }

        public bool HasAccess { get; set; }

        public string Name
        {
            get { return User.DisplayUserName(false, DisplayUserSettingsHelper); }
        }

        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }

        public UserInfo User { get; set; }

        public MentionWrapper(UserInfo user, DisplayUserSettingsHelper displayUserSettingsHelper)
        {
            User = user;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
        }
    }

    public class MentionMessageWrapper
    {
        public ActionLinkConfig ActionLink { get; set; }

        public List<string> Emails { get; set; }

        public string Message { get; set; }
    }
}