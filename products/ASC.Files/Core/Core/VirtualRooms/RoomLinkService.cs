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

namespace ASC.Files.Core.VirtualRooms;

[Scope]
public class RoomLinkService
{
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly DocSpaceLinkHelper _docSpaceLinksHelper;

    public RoomLinkService(CommonLinkUtility commonLinkUtility, IDaoFactory daoFactory, DocSpaceLinkHelper docSpaceLinksHelper)
    {
        _commonLinkUtility = commonLinkUtility;
        _daoFactory = daoFactory;
        _docSpaceLinksHelper = docSpaceLinksHelper;
    }

    public string GetInvitationLink(Guid linkId, Guid createdBy)
    {
        var key = _docSpaceLinksHelper.MakeKey(linkId);

        return _commonLinkUtility.GetConfirmationUrl(key, ConfirmType.LinkInvite, createdBy);
    }

    public string GetInvitationLink(string email, FileShare share, Guid createdBy)
    {
        var type = DocSpaceHelper.PaidRights.Contains(share) ? EmployeeType.User : EmployeeType.Visitor;

        var link = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.LinkInvite, type, createdBy)
            + $"&emplType={type:d}";

        return link;
    }

    public string GetInvitationLink(string email, EmployeeType employeeType, Guid createdBy)
    {
        var link = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.LinkInvite, employeeType, createdBy)
            + $"&emplType={employeeType:d}";

        return link;
    }

    public async Task<LinkOptions> GetOptionsAsync(string key, string email)
    {
        return await GetOptionsAsync(key, email, EmployeeType.All);
    }

    public async Task<LinkOptions> GetOptionsAsync(string key, string email, EmployeeType employeeType)
    {
        var options = new LinkOptions();

        var payload = _docSpaceLinksHelper.Parse(key);

        if (payload != default)
        {
            var record = await GetRecordAsync(payload);

            if (record != null)
            {
                options.IsCorrect = true;
                options.LinkType = LinkType.InvintationToRoom;
                options.RoomId = record.EntryId.ToString();
                options.Share = record.Share;
                options.Id = record.Subject;
                options.EmployeeType = DocSpaceHelper.PaidRights.Contains(record.Share) ? EmployeeType.User : EmployeeType.Visitor;
            }
        }
        else if (_docSpaceLinksHelper.ValidateEmailLink(email, key, employeeType) == EmailValidationKeyProvider.ValidationResult.Ok)
        {
            options.IsCorrect = true;
            options.LinkType = LinkType.InvintationByEmail;
            options.EmployeeType = employeeType;
        }
        else if (_docSpaceLinksHelper.ValidateExtarnalLink(key, employeeType) == EmailValidationKeyProvider.ValidationResult.Ok)
        {
            options.LinkType = LinkType.DefaultInvintation;
            options.IsCorrect = true;
            options.EmployeeType = employeeType;
        }

        return options;
    }

    private async Task<FileShareRecord> GetRecordAsync(Guid key)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var share = await securityDao.GetSharesAsync(new[] { key })
            .Where(s => s.SubjectType == SubjectType.InvintationLink)
            .FirstOrDefaultAsync();

        return share;
    }
}

public class LinkOptions
{
    public Guid Id { get; set; }
    public string RoomId { get; set; }
    public FileShare Share { get; set; }
    public LinkType LinkType { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public bool IsCorrect { get; set; }
}

[EnumExtensions]
public enum LinkType
{
    DefaultInvintation,
    InvintationByEmail,
    InvintationToRoom,
}