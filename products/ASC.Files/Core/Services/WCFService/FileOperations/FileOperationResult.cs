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

namespace ASC.Web.Files.Services.WCFService.FileOperations;

[ProtoContract]
[ProtoInclude(100, typeof(FileConverterOperationResult))]
public class FileOperationResult
{
    [ProtoMember(1)]
    public string Id { get; set; }

    [JsonPropertyName("operation")]
    [ProtoMember(2)]
    public FileOperationType OperationType { get; set; }

    [ProtoMember(3)]
    public int Progress { get; set; }

    [ProtoMember(4)]
    public string Source { get; set; }

    [ProtoMember(5)]
    public string Result { get; set; }

    [ProtoMember(6)]
    public string Error { get; set; }

    [ProtoMember(7)]
    public string Processed { get; set; }

    [ProtoMember(8)]
    public bool Finished { get; set; }
}
