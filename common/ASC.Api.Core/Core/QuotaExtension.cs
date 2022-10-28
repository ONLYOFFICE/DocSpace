﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Api.Core.Core;
public static class QuotaExtension
{
    public static void RegisterFeature(this IServiceCollection services)
    {
        services.AddScoped<ITenantQuotaFeatureChecker, CountRoomAdminChecker>();
        services.AddScoped<TenantQuotaFeatureCheckerCount<CountRoomAdminFeature>, CountRoomAdminChecker>();
        services.AddScoped<CountRoomAdminChecker>();    
        services.AddScoped<ITenantQuotaFeatureStat<CountRoomAdminFeature, int>, CountRoomAdminStatistic>();
        services.AddScoped<CountRoomAdminStatistic>();

        services.AddScoped<ITenantQuotaFeatureChecker, CountUserChecker>();
        services.AddScoped<TenantQuotaFeatureCheckerCount<CountUserFeature>, CountUserChecker>();
        services.AddScoped<CountUserChecker>();
        services.AddScoped<ITenantQuotaFeatureStat<CountUserFeature, int>, CountUserStatistic>();
        services.AddScoped<CountUserStatistic>();

        services.AddScoped<ITenantQuotaFeatureChecker, MaxTotalSizeChecker>();
        services.AddScoped<TenantQuotaFeatureChecker<MaxTotalSizeFeature, long>, MaxTotalSizeChecker>();
        services.AddScoped<MaxTotalSizeChecker>();
        services.AddScoped<ITenantQuotaFeatureStat<MaxTotalSizeFeature, long>, MaxTotalSizeStatistic>();
        services.AddScoped<MaxTotalSizeStatistic>();

        services.AddScoped<TenantQuotaFeatureChecker<MaxFileSizeFeature, long>, MaxFileSizeChecker>();
        services.AddScoped<ITenantQuotaFeatureStat<MaxFileSizeFeature, long>, MaxFileSizeStatistic>();
    }
}
