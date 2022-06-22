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

/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace ASC.Files.Core;

public enum DateToAutoCleanUp
{
    OneWeek = 1,
    TwoWeeks,
    OneMonth,
    TwoMonths,
    ThreeMonths
}

public class AutoCleanUpData
{
    public bool IsAutoCleanUp { get; set; }

    public DateToAutoCleanUp Gap { get; set; }
}

[Scope]
public class FileDateTime
{
    private readonly TenantUtil _tenantUtil;

    public FileDateTime(TenantUtil tenantUtil)
    {
        _tenantUtil = tenantUtil;
    }

    public DateTime GetModifiedOnWithAutoCleanUp(DateTime modifiedOn, DateToAutoCleanUp date, bool utc = false)
    {
        var dateTime = modifiedOn;
        switch (date)
        {
            case DateToAutoCleanUp.OneWeek: dateTime = dateTime.AddDays(7); break;
            case DateToAutoCleanUp.TwoWeeks: dateTime = dateTime.AddDays(14); break;
            case DateToAutoCleanUp.OneMonth: dateTime = dateTime.AddMonths(1); break;
            case DateToAutoCleanUp.TwoMonths: dateTime = dateTime.AddMonths(2); break;
            case DateToAutoCleanUp.ThreeMonths: dateTime = dateTime.AddMonths(3); break;
            default: break;
        }
        return utc ? _tenantUtil.DateTimeToUtc(dateTime) : dateTime;
    }
}