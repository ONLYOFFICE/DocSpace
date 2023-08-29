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

namespace ASC.ApiSystem.Models;

public class TenantModel : IModel
{
    public string PortalName { get; set; }

    public int? TenantId { get; set; }

    [StringLength(255)]
    public string AffiliateId { get; set; }

    [StringLength(255)]
    public string PartnerId { get; set; }

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
    [StringLength(PasswordSettings.MaxLength)]
    public string Password { get; set; }

    public string PasswordHash { get; set; }

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
}
