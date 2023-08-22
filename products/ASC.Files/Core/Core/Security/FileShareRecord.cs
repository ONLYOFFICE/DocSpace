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

namespace ASC.Files.Core.Security;

public class FileShareRecord
{
    public int TenantId { get; set; }
    public object EntryId { get; set; }
    public FileEntryType EntryType { get; set; }
    public SubjectType SubjectType { get; set; }
    public Guid Subject { get; set; }
    public Guid Owner { get; set; }
    public FileShare Share { get; set; }
    public FileShareOptions Options { get; set; }
    public int Level { get; set; }
    public bool IsLink => SubjectType is SubjectType.InvitationLink or SubjectType.ExternalLink;

    public class ShareComparer : IComparer<FileShare>
    {
        private static readonly int[] _shareOrder = new[]
        {
                (int)FileShare.None,
                (int)FileShare.ReadWrite,
                (int)FileShare.CustomFilter,
                (int)FileShare.Review,
                (int)FileShare.FillForms,
                (int)FileShare.Comment,
                (int)FileShare.Read,
                (int)FileShare.Restrict,
                (int)FileShare.Varies
        };

        public int Compare(FileShare x, FileShare y)
        {
            return Array.IndexOf(_shareOrder, (int)x).CompareTo(Array.IndexOf(_shareOrder, (int)y));
        }
    }
}

public class SmallShareRecord
{
    public Guid Subject { get; set; }
    public Guid ShareParentTo { get; set; }
    public Guid Owner { get; set; }
    public DateTime TimeStamp { get; set; }
    public FileShare Share { get; set; }
    public SubjectType SubjectType { get; set; }
}
