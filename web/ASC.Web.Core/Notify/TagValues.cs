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

namespace ASC.Web.Studio.Core.Notify;

public static class TagValues
{
    public static ITagValue WithoutUnsubscribe()
    {
        return new TagValue(CommonTags.WithoutUnsubscribe, true);
    }

    public static ITagValue PersonalHeaderStart()
    {
        return new TagValue("PersonalHeaderStart",
                            "<table style=\"margin: 0; border-spacing: 0; empty-cells: show; width: 540px; width: 100%;\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr><td style=\"width: 100%;color: #333333;font-size: 18px;font-weight: bold;margin: 0;height: 71px;padding-right:165px;background: url('https://d2nlctn12v279m.cloudfront.net/media/newsletters/images/personal-header-bg.png') top right no-repeat;\">");
    }

    public static ITagValue PersonalHeaderEnd()
    {
        return new TagValue("PersonalHeaderEnd", "</td></tr></tbody></table>");
    }

    public static ITagValue BlueButton(Func<string> btnTextFunc, string btnUrl)
    {
        string action()
        {
            var btnText = btnTextFunc != null ? btnTextFunc() ?? string.Empty : string.Empty;
            const string td = "<td style=\"height: 48px; width: 80px; margin:0; padding:0;\">&nbsp;</td>";
            const string color = "color: #fff; font-family: Helvetica, Arial, Tahoma; font-size: 18px; font-weight: 600; vertical-align: middle; display: block; padding: 12px 0; text-align: center; text-decoration: none; background-color: #66b76d;";

            return $@"<table style=""height: 48px; width: 540px; border-collapse: collapse; empty-cells: show; vertical-align: middle; text-align: center; margin: 30px auto; padding: 0;""><tbody><tr cellpadding=""0"" cellspacing=""0"" border=""0"">{td}<td style=""height: 48px; width: 380px; margin:0; padding:0; background-color: #66b76d; -moz-border-radius: 2px; -webkit-border-radius: 2px; border-radius: 2px;""><a style=""{color}"" target=""_blank"" href=""{btnUrl}"">{btnText}</a></td>{td}</tr></tbody></table>";
        }

        return new TagActionValue("BlueButton", action);
    }

    public static ITagValue GreenButton(Func<string> btnTextFunc, string btnUrl)
    {
        string action()
        {
            var btnText = btnTextFunc != null ? btnTextFunc() ?? string.Empty : string.Empty;
            const string td = "<td style=\"height: 48px; width: 80px; margin:0; padding:0;\">&nbsp;</td>";
            const string color = "color: #fff; font-family: Helvetica, Arial, Tahoma; font-size: 18px; font-weight: 600; vertical-align: middle; display: block; padding: 12px 0; text-align: center; text-decoration: none; background-color: #66b76d;";

            return $@"<table style=""height: 48px; width: 540px; border-collapse: collapse; empty-cells: show; vertical-align: middle; text-align: center; margin: 30px auto; padding: 0;""><tbody><tr cellpadding=""0"" cellspacing=""0"" border=""0"">{td}<td style=""height: 48px; width: 380px; margin:0; padding:0; background-color: #66b76d; -moz-border-radius: 2px; -webkit-border-radius: 2px; border-radius: 2px;""><a style=""{color}"" target=""_blank"" href=""{btnUrl}"">{btnText}</a></td>{td}</tr></tbody></table>";
        }

        return new TagActionValue("GreenButton", action);
    }

    public static ITagValue TableTop()
    {
        return new TagValue("TableItemsTop",
                            "<table cellpadding=\"0\" cellspacing=\"0\" style=\"margin: 20px 0 0; border-spacing: 0; empty-cells: show; width: 540px; font-size: 14px;\">");
    }

    public static ITagValue TableBottom()
    {
        return new TagValue("TableItemsBtm", "</table>");
    }

    public static ITagValue TableItem(
        int number,
        Func<string> linkTextFunc,
        string linkUrl,
        string imgSrc,
        Func<string> commentFunc,
        Func<string> bottomlinkTextFunc,
        string bottomlinkUrl)
    {
        string action()
        {
            var linkText = linkTextFunc != null ? linkTextFunc() ?? string.Empty : string.Empty;
            var comment = commentFunc != null ? commentFunc() ?? string.Empty : string.Empty;
            var bottomlinkText = bottomlinkTextFunc != null
                                     ? bottomlinkTextFunc() ?? string.Empty
                                     : string.Empty;

            var imgHtml = $"<img style=\"border: 0; padding: 0 15px 0 5px; width: auto; height: auto;\" alt=\"{linkText}\" src=\"{imgSrc ?? string.Empty}\"/>";

            var linkHtml = string.Empty;

            if (!string.IsNullOrEmpty(linkText))
            {
                linkHtml =
                    !string.IsNullOrEmpty(linkUrl)
                        ? $"<a target=\"_blank\" style=\"color:#0078bd; font-family: Arial; font-size: 14px; font-weight: bold;\" href=\"{linkUrl}\">{linkText}</a><br/>"
                        : $"<div style=\"display:block; color:#333333; font-family: Arial; font-size: 14px; font-weight: bold;margin-bottom: 10px;\">{linkText}</div>";
            }

            var underCommentLinkHtml =
                string.IsNullOrEmpty(bottomlinkUrl)
                    ? string.Empty
                    : $"<br/><a target=\"_blank\" style=\"color: #0078bd; font-family: Arial; font-size: 14px;\" href=\"{bottomlinkUrl}\">{bottomlinkText}</a>";

            var html =
                "<tr><td style=\"vertical-align: top; padding: 5px 10px; width: 70px;\">" +
                imgHtml +
                "</td><td style=\" vertical-align: middle; padding: 5px 10px; font-size: 14px; width: 470px; color: #333333;\">" +
                linkHtml +
                comment +
                underCommentLinkHtml +
                "</td></tr>";

            return html;
        }

        return new TagActionValue("TableItem" + number, action);
    }

    public static ITagValue Image(StudioNotifyHelper studioNotifyHelper, int id, string imageFileName)
    {
        var imgSrc = studioNotifyHelper.GetNotificationImageUrl(imageFileName);

        var imgHtml = $"<img style=\"border: 0; padding: 0; width: auto; height: auto;\" alt=\"\" src=\"{imgSrc}\"/>";

        var tagName = "Image" + (id > 0 ? id.ToString() : string.Empty);

        return new TagValue(tagName, imgHtml);
    }
}
