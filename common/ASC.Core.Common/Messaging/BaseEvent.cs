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

using Profile = AutoMapper.Profile;

namespace ASC.AuditTrail.Models;

public class BaseEvent : IMapFrom<DbLoginEvent>
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public bool Mobile { get; set; }
    public IList<string> Description { get; set; }

    [Event("IpCol")]
    public string IP { get; set; }

    [Event("CountryCol")]
    public string Country { get; set; }

    [Event("CityCol")]
    public string City { get; set; }

    [Event("BrowserCol")]
    public string Browser { get; set; }

    [Event("PlatformCol")]
    public string Platform { get; set; }

    [Event("DateCol")]
    public DateTime Date { get; set; }

    [Event("UserCol")]
    public string UserName { get; set; }

    [Event("PageCol")]
    public string Page { get; set; }

    [Event("ActionCol")]
    public string ActionText { get; set; }

    public virtual void Mapping(Profile profile)
    {
        profile.CreateMap<DbLoginEvent, BaseEvent>()
            .ForMember(r => r.IP, opt => opt.MapFrom<BaseEventTypeIpResolver>())
            .ForMember(r => r.Date, opt => opt.MapFrom<BaseEventTypeDateResolver>())
            ;
    }
}

