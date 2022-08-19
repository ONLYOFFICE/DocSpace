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

using ASC.AuditTrail.Types;

using ActionType = ASC.AuditTrail.Types.ActionType;
using Profile = AutoMapper.Profile;

namespace ASC.Files.Core.ApiModels.ResponseDto;

public class RoomEventDto : IMapFrom<AuditEventDto>
{
    public int Id { get; set; }
    public ApiDateTime Date { get; set; }
    public string UserName { get; set; }
    public Guid UserId { get; set; }
    public string ActionText { get; set; }
    public int Action { get; set; }
    public string IP { get; set; }
    public string Browser { get; set; }
    public string Platform { get; set; }
    public string Page { get; set; }
    public ActionType ActionType { get; set; }
    public ProductType ProductType { get; set; }
    public ModuleType ModuleType { get; set; }
    public IEnumerable<string> Targets { get; set; }
    public IEnumerable<EntryType> Entries { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AuditEventDto, RoomEventDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom((src, dest, _) => dest.Date = new ApiDateTime(src.Date, TimeSpan.Zero)))
            .AfterMap<RoomEventMappingAction>();
    }
}