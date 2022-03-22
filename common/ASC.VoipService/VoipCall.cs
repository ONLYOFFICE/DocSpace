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

namespace ASC.VoipService;

public class VoipCall : IMapFrom<CallContact>
{
    public string Id { get; set; }
    public string ParentCallId { get; set; }
    public string NumberFrom { get; set; }
    public string NumberTo { get; set; }
    public Guid AnsweredBy { get; set; }
    public DateTime DialDate { get; set; }
    public int DialDuration { get; set; }
    public VoipCallStatus? Status { get; set; }
    public decimal Price { get; set; }
    public int ContactId { get; set; }
    public bool ContactIsCompany { get; set; }
    public string ContactTitle { get; set; }
    public DateTime Date { get; set; }
    public DateTime EndDialDate { get; set; }
    public VoipRecord VoipRecord { get; set; }
    public List<VoipCall> ChildCalls { get; set; }

    public VoipCall()
    {
        ChildCalls = new List<VoipCall>();
        VoipRecord = new VoipRecord();
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbVoipCall, VoipCall>();

        profile.CreateMap<CallContact, VoipCall>()
            .ConvertUsing<CallTypeConverter>();
    }
}

public enum VoipCallStatus
{
    Incoming,
    Outcoming,
    Answered,
    Missed
}