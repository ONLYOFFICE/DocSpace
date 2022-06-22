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

namespace ASC.ApiSystem.Models;

public class TenantModel : IModel
{
    public string PortalName { get; set; }

    public int? TenantId { get; set; }

    [StringLength(255)]
    public string AffiliateId { get; set; }

    public string Campaign { get; set; }

    [StringLength(255)]
    public string FirstName { get; set; }

    //todo  [Email]
    [StringLength(255)]
    public string Email { get; set; }

    public int Industry { get; set; }

    [StringLength(7)]
    public string Language { get; set; }

    [StringLength(255)]
    public string LastName { get; set; }

    [StringLength(38)]
    public string Module { get; set; }

    //todo: delete after www update
    [StringLength(Web.Core.Utility.PasswordSettings.MaxLength)]
    public string Password { get; set; }

    public string PasswordHash { get; set; }

    [StringLength(255)]
    public string PartnerId { get; set; }

    [StringLength(32)]
    public string Phone { get; set; }

    public string RecaptchaResponse { get; set; }

    public RecaptchaType RecaptchaType { get; set; }

    [StringLength(20)]
    public string Region { get; set; }

    public TenantStatus Status { get; set; }

    public bool SkipWelcome { get; set; }

    [StringLength(255)]
    public string TimeZoneName { get; set; }

    public bool Spam { get; set; }

    public bool Calls { get; set; }

    public string AppKey { get; set; }

    public bool LimitedControlPanel { get; set; }
}
