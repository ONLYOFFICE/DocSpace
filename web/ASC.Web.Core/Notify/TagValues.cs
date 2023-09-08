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

    public static ITagValue GreenButton(string btnTextFunc, string btnUrl)
    {
        var btnText = btnTextFunc ?? string.Empty;
        const string td = "<td style=\"height: 48px; width: 80px; margin:0; padding:0;\">&nbsp;</td>";
        const string color = "color: #fff; font-family: Helvetica, Arial, Tahoma; font-size: 18px; font-weight: 600; vertical-align: middle; display: block; padding: 12px 0; text-align: center; text-decoration: none; background-color: #66b76d;";

        var action = $@"<table style=""height: 48px; width: 540px; border-collapse: collapse; empty-cells: show; vertical-align: middle; text-align: center; margin: 30px auto; padding: 0;""><tbody><tr cellpadding=""0"" cellspacing=""0"" border=""0"">{td}<td style=""height: 48px; width: 380px; margin:0; padding:0; background-color: #66b76d; -moz-border-radius: 2px; -webkit-border-radius: 2px; border-radius: 2px;""><a style=""{color}"" target=""_blank"" href=""{btnUrl}"">{btnText}</a></td>{td}</tr></tbody></table>";

        return new TagValue("GreenButton", action);
    }
    public static ITagValue OrangeButton(string btnTextFunc, string btnUrl)
    {
        var btnText = btnTextFunc ?? string.Empty;
        const string td = "<td style=\"height: 48px; width: 80px; margin:0; padding:0;\">&nbsp;</td>";
        const string color = "background-color:#FF6F3D; border:1px solid #FF6F3D; border-radius: 3px; color:#ffffff; display: inline-block; font-family: 'Open Sans', Helvetica, Arial, Tahoma, sans-serif; font-size: 13px; font-weight: 600; padding-top: 15px; padding-right: 25px; padding-bottom: 15px; padding-left: 25px; text-align: center; text-decoration: none; text-transform: uppercase; -webkit-text-size-adjust: none; mso-hide: all; white-space: nowrap; letter-spacing: 0.04em;";

        var action = $@"<table style=""border: 0 none; border-collapse: collapse; border-spacing: 0; empty-cells: show; Margin: 0 auto; max-width: 600px; padding: 0; vertical-align: top; width: 100%; text-align: left;""><tbody><tr cellpadding=""0"" cellspacing=""0"" border=""0"">{td}<td style=""height: 48px; width: 380px; margin:0; padding:0; text-align:center;""><a style=""{color}"" target=""_blank"" href=""{btnUrl}"">{btnText}</a></td>{td}</tr></tbody></table>";

        return new TagValue("OrangeButton", action);
    }

    public static ITagValue TrulyYours()
    {
        var txtTrulyYours = WebstudioNotifyPatternResource.TrulyYoursText;
        const string tdStyle = "color: #333333; font-family: 'Open Sans', Helvetica, Arial, Tahoma, sans-serif; font-size: 14px; line-height: 1.6em; Margin: 0; padding: 0px 190px 40px; vertical-align: top; text-align: center;";
        const string astyle = "color: #FF6F3D; text-decoration: none;";
        var action = $@"<tr border=""0"" cellspacing=""0"" cellpadding=""0""><td class=""fol"" style=""{tdStyle}"">{txtTrulyYours} <br /><a style=""{astyle}"" target=""_blank"" href=""https://www.onlyoffice.com/"">www.onlyoffice.com</a></td></tr>";
        return new TagValue("TrulyYours", action);
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

    public static ITagValue Image(StudioNotifyHelper studioNotifyHelper, int id, string imageFileName)
    {
        var imgSrc = studioNotifyHelper.GetNotificationImageUrl(imageFileName);

        var imgHtml = $"<img style=\"border: 0; padding: 0; width: auto; height: auto;\" alt=\"\" src=\"{imgSrc}\"/>";

        var tagName = "Image" + (id > 0 ? id.ToString() : string.Empty);

        return new TagValue(tagName, imgHtml);
    }
}
