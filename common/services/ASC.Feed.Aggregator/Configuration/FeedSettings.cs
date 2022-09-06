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

namespace ASC.Feed.Configuration;

[Singletone]
public class FeedSettings
{
    public string ServerRoot
    {
        get => _serverRoot ?? "http://*/";
        set => _serverRoot = value;
    }
    public TimeSpan AggregatePeriod
    {
        get => _aggregatePeriod == TimeSpan.Zero ? TimeSpan.FromMinutes(5) : _aggregatePeriod;
        set => _aggregatePeriod = value;
    }
    public TimeSpan AggregateInterval
    {
        get => _aggregateInterval == TimeSpan.Zero ? TimeSpan.FromDays(14) : _aggregateInterval;
        set => _aggregateInterval = value;
    }
    public TimeSpan RemovePeriod
    {
        get => _removePeriod == TimeSpan.Zero ? TimeSpan.FromDays(1) : _removePeriod;
        set => _removePeriod = value;
    }

    private string _serverRoot;
    private TimeSpan _aggregatePeriod;
    private TimeSpan _aggregateInterval;
    private TimeSpan _removePeriod;

    public FeedSettings(ConfigurationExtension configuration)
    {
        configuration.GetSetting("feed", this);
    }
}
