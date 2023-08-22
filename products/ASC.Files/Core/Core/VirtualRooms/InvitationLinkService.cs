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

using ASC.Core.Billing;

namespace ASC.Files.Core.VirtualRooms;

[Scope]
public class InvitationLinkService
{
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly InvitationLinkHelper _invitationLinkHelper;
    private readonly ITariffService _tariffService;
    private readonly TenantManager _tenantManager;
    private readonly AuthManager _authManager;
    private readonly PermissionContext _permissionContext;
    private readonly CountPaidUserChecker _countPaidUserChecker;

    public InvitationLinkService(
        CommonLinkUtility commonLinkUtility, 
        IDaoFactory daoFactory, 
        InvitationLinkHelper invitationLinkHelper, 
        ITariffService tariffService, 
        TenantManager tenantManager,
        AuthManager authManager, 
        PermissionContext permissionContext, 
        CountPaidUserChecker countPaidUserChecker)
    {
        _commonLinkUtility = commonLinkUtility;
        _daoFactory = daoFactory;
        _invitationLinkHelper = invitationLinkHelper;
        _tariffService = tariffService;
        _tenantManager = tenantManager;
        _authManager = authManager;
        _permissionContext = permissionContext;
        _countPaidUserChecker = countPaidUserChecker;
    }

    public string GetInvitationLink(Guid linkId, Guid createdBy)
    {
        var key = _invitationLinkHelper.MakeIndividualLinkKey(linkId);

        return _commonLinkUtility.GetConfirmationUrl(key, ConfirmType.LinkInvite, createdBy) + "&toRoom=true";
    }

    public async Task<string> GetInvitationLinkAsync(string email, FileShare share, Guid createdBy)
    {
        var type = FileSecurity.GetTypeByShare(share);
        
        var link = await _commonLinkUtility.GetConfirmationEmailUrlAsync(email, ConfirmType.LinkInvite, type, createdBy)
                   + $"&emplType={type:d}&toRoom=true";

        return link;
    }

    public async Task<string> GetInvitationLinkAsync(string email, EmployeeType employeeType, Guid createdBy)
    {
        var link = await _commonLinkUtility.GetConfirmationEmailUrlAsync(email, ConfirmType.LinkInvite, employeeType, createdBy)
            + $"&emplType={employeeType:d}";

        return link;
    }

    public async Task<InvitationLinkData> GetProcessedLinkDataAsync(string key, string email)
    {
        return await GetProcessedLinkDataAsync(key, email, EmployeeType.All);
    }

    public async Task<InvitationLinkData> GetProcessedLinkDataAsync(string key, string email, EmployeeType employeeType, Guid userId = default)
    {
        Tenant tenant;
        var linkData = new InvitationLinkData { Result = EmailValidationKeyProvider.ValidationResult.Invalid };

        try
        {
            tenant = await _tenantManager.GetCurrentTenantAsync();
        }
        catch (Exception)
        {
            return linkData;
        }

        if ((await _tariffService.GetTariffAsync(tenant.Id)).State > TariffState.Paid)
        {
            return new InvitationLinkData { Result = EmailValidationKeyProvider.ValidationResult.Invalid };
        }
        
        if (userId != default)
        {
            var account = await _authManager.GetAccountByIDAsync(tenant.Id, userId);

            if (!await _permissionContext.CheckPermissionsAsync(account, new UserSecurityProvider(employeeType), Constants.Action_AddRemoveUser))
            {
                return linkData;
            }
        }

        var validationResult = await _invitationLinkHelper.ValidateAsync(key, email, employeeType);
        linkData.Result = validationResult.Result;
        linkData.LinkType = validationResult.LinkType;
        linkData.EmployeeType = employeeType;


        if (validationResult.LinkId == default)
        {
            if (!await CheckQuota(linkData.LinkType, employeeType))
            {
                linkData.Result = EmailValidationKeyProvider.ValidationResult.Invalid;
            }
            
            return linkData;
        }

        var record = await GetLinkRecordAsync(validationResult.LinkId);

        if (record?.Options == null)
        {
            linkData.Result = EmailValidationKeyProvider.ValidationResult.Invalid;
            return linkData;
        }

        linkData.Result = record.Options.ExpirationDate > DateTime.UtcNow ? 
            EmailValidationKeyProvider.ValidationResult.Ok : EmailValidationKeyProvider.ValidationResult.Expired;
        linkData.Share = record.Share;
        linkData.RoomId = record.EntryId.ToString();
        linkData.EmployeeType = FileSecurity.GetTypeByShare(record.Share);

        if (!await CheckQuota(linkData.LinkType, linkData.EmployeeType))
        {
            linkData.Result = EmailValidationKeyProvider.ValidationResult.Invalid;
        }

        return linkData;
    }

    private async Task<FileShareRecord> GetLinkRecordAsync(Guid linkId)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var share = await securityDao.GetSharesAsync(new[] { linkId })
            .FirstOrDefaultAsync(s => s.SubjectType == SubjectType.InvitationLink);

        return share;
    }

    private async Task<bool> CheckQuota(InvitationLinkType linkType, EmployeeType employeeType)
    {
        if (linkType == InvitationLinkType.Individual ||
            employeeType is not (EmployeeType.DocSpaceAdmin or EmployeeType.RoomAdmin or EmployeeType.Collaborator))
        {
            return true;
        }

        try
        {
            await _countPaidUserChecker.CheckAppend();
        }
        catch (TenantQuotaException)
        {
            return false;
        }

        return true;
    }
}

public class InvitationLinkData
{
    public string RoomId { get; set; }
    public FileShare Share { get; set; }
    public InvitationLinkType LinkType { get; set; }
    public EmployeeType EmployeeType { get; set; }
    public EmailValidationKeyProvider.ValidationResult Result { get; set; }
    public bool IsCorrect => Result == EmailValidationKeyProvider.ValidationResult.Ok;
}