// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.ActiveDirectory.Base;

public static class NotifyConstants
{
    public static string TagUserName = "UserName";
    public static string TagUserEmail = "UserEmail";
    public static string TagMyStaffLink = "MyStaffLink";

    public static INotifyAction ActionLdapActivation = new NotifyAction("user_ldap_activation");

    public static ITagValue TagGreenButton(string btnText, string btnUrl)
    {
        Func<string> action = () =>
        {
            return
                string.Format(@"<table style=""height: 48px; width: 540px; border-collapse: collapse; empty-cells: show; vertical-align: middle; text-align: center; margin: 30px auto; padding: 0;""><tbody><tr cellpadding=""0"" cellspacing=""0"" border=""0"">{2}<td style=""height: 48px; width: 380px; margin:0; padding:0; background-color: #66b76d; -moz-border-radius: 2px; -webkit-border-radius: 2px; border-radius: 2px;""><a style=""{3}"" target=""_blank"" href=""{0}"">{1}</a></td>{2}</tr></tbody></table>",
                    btnUrl,
                    btnText,
                    "<td style=\"height: 48px; width: 80px; margin:0; padding:0;\">&nbsp;</td>",
                    "color: #fff; font-family: Helvetica, Arial, Tahoma; font-size: 18px; font-weight: 600; vertical-align: middle; display: block; padding: 12px 0; text-align: center; text-decoration: none; background-color: #66b76d;");
        };
        return new TagActionValue("GreenButton", action);
    }

    private class TagActionValue : ITagValue
    {
        private readonly Func<string> action;

        public string Tag
        {
            get;
            private set;
        }

        public object Value
        {
            get { return action(); }
        }

        public TagActionValue(string name, Func<string> action)
        {
            Tag = name;
            this.action = action;
        }
    }
}

public static class NotifyCommonTags
{
    public static string Footer = "Footer";

    public static string MasterTemplate = "MasterTemplate";

    public static string WithoutUnsubscribe = "WithoutUnsubscribe";
}
